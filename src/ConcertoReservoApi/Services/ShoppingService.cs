using ConcertoReservoApi.Core;
using ConcertoReservoApi.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConcertoReservoApi.Controllers.ShoppingController;
using static ConcertoReservoApi.Core.EventSeat;
using static ConcertoReservoApi.Services.IShoppingService;

namespace ConcertoReservoApi.Services
{
    public interface IShoppingService
    {
        enum ShoppingErrors
        {
            NotFound,
            DuplicateSessionCreated,
            TechnicalError,
            SelectedSeatReserved,
            UnacceptablyBadInput,
            CannotCheckoutWithValidationIssues
        }
        record Result<T>(T Data, ShoppingErrors? Error = null);
        record Result(ShoppingErrors? Error);

        /// <summary>
        /// creates a shopping session
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Result<ShoppingSessionView> StartShopping(string eventId);
        /// <summary>
        /// gets a shopping session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<ShoppingSessionView> GetSession(string id);
        /// <summary>
        /// updates shopper information for an existing session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shopper"></param>
        /// <returns></returns>
        Result<ShoppingSessionView> UpdateShopper(string id, ShopperDto shopper);
        /// <summary>
        /// gets the available seating for a shopping session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<AvailableEventSeatsView> GetAvailableSeating(string id);
        /// <summary>
        /// reserves seating for a shopping session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        Result<ShoppingSessionView> SelectSeating(string id, SeatSelectionDto[] selection);
        /// <summary>
        /// updates payment information from vendor, validates against ventor endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paymentTokenizationId"></param>
        /// <returns></returns>
        Result<ShoppingSessionView> UpdatePaymentInformation(string id, string paymentTokenizationId);
        /// <summary>
        /// attempts to campture payment and mark seats as sold
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result AttemptPurchase(string id);
    }

    public class ShoppingService : IShoppingService
    {
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<ShoppingService> _logger;
        private readonly ISeatingRepository _seatingRepository;
        private readonly IPaymentService _paymentService;

        public ShoppingService(IShoppingRepository shoppingRepository, ILogger<ShoppingService> logger, IEventsRepository eventsRepository, ISeatingRepository seatingRepository, IPaymentService paymentService)
        {
            _shoppingRepository = shoppingRepository;
            _logger = logger;
            _eventsRepository = eventsRepository;
            _seatingRepository = seatingRepository;
            _paymentService = paymentService;
        }

        public Result<ShoppingSessionView> StartShopping(string eventId)
        {
            //would normally validate against duplicate creation with infrastructure inputs like the ip, browser, and other data
            var eventInfo = _eventsRepository.GetEvent(eventId);
            if (eventInfo == null)
            {
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);
            }

            var session = _shoppingRepository.CreateShoppingSession(eventId);
            if (session == null)
            {
                return new Result<ShoppingSessionView>(null, ShoppingErrors.TechnicalError);
            }

            session.Validate();
            return new Result<ShoppingSessionView>(session.ToDto());
        }

        public Result<ShoppingSessionView> GetSession(string id)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            session.Validate();
            return new Result<ShoppingSessionView>(session.ToDto());
        }

        public Result<AvailableEventSeatsView> GetAvailableSeating(string id)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<AvailableEventSeatsView>(null, ShoppingErrors.NotFound);

            //ideally this would be a domain service of some sort that takes more into account from the shopper, like affiliation and what not, going with something simple like avaiable seating
            var availableSeating = _seatingRepository.GetEventSeating(session.EventId);
            if (availableSeating == null)
            {
                return new Result<AvailableEventSeatsView>(null, ShoppingErrors.NotFound);
            }

            new AvailableEventSeatsView
            {
                Sections = _eventsRepository
            }

            //build dto from seating obj
            throw new NotImplementedException();
            //return new Result<AvailableEventSeatsView>(dto);
        }

        /// <summary>
        /// looks up selected seats, ensures they're all available, adds them to shopping session, reserves them, then saves session. if anything goes wrong it attempts to remove seats from session and clear any reserved seating.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public Result<ShoppingSessionView> SelectSeating(string id, SeatSelectionDto[] selection)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            session.ClearSelectedSeats();
            foreach (var selectedSeat in selection)
            {
                var eventSeat = _seatingRepository.GetSeat(session.EventId, selectedSeat.SeatId);
                if (eventSeat.Status != SeatReservationStatuses.Available)
                    return new Result<ShoppingSessionView>(null, ShoppingErrors.SelectedSeatReserved);

                session.AddSelectedSeating(eventSeat, selectedSeat.AddonIds);
            }

            var anyReservationFailed = false;
            foreach (var seat in session.SelectedSeats)
            {
                anyReservationFailed = anyReservationFailed || !_seatingRepository.ReserveSeat(session.EventId, session.Id, seat);
            }

            //I don't like the early return pattern, but didn't want to duplicate unreservation block, could use a second pass here
            if (!anyReservationFailed)
            {
                session.EnsureCountdownStarted();
                var saved = _shoppingRepository.Save(session); //if this fails we should also remove reservations

                if (saved)
                {
                    session.Validate();
                    return new Result<ShoppingSessionView>(session.ToDto());
                }
            }

            //this is somewhat unnecessary as the seat reservation should expire when the shopping session expires if it was left dangling, but it would ruin that seat for this and all other users as it's no longer attached to the session
            foreach (var seat in session.SelectedSeats)
            {
                _seatingRepository.RemoveReservation(session.EventId, session.Id, seat);
            }
            return new Result<ShoppingSessionView>(null, ShoppingErrors.SelectedSeatReserved);
        }

        public Result<ShoppingSessionView> UpdateShopper(string id, ShopperDto shopper)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            session.UpdateShopper(
                shopper.Email,
                shopper.FullName,
                shopper.PhoneNumber,
                shopper.AffilateCodes);

            _shoppingRepository.Save(session);

            session.Validate();
            return new Result<ShoppingSessionView>(session.ToDto());
        }

        public Result<ShoppingSessionView> UpdatePaymentInformation(string id, string paymentTokenizationId)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            var valid = _paymentService.ValidatePaymentToken(paymentTokenizationId);
            if (!valid)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.UnacceptablyBadInput);

            session.AttachPaymentToken(paymentTokenizationId);

            _shoppingRepository.Save(session);

            session.Validate();
            return new Result<ShoppingSessionView>(session.ToDto());
        }

        public Result AttemptPurchase(string id)
        {
            //improvements to make, chunk into commands that have state, idempotency in each destructive command (payment)

            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result(ShoppingErrors.NotFound);

            session.Validate();
            if (session.ValidationIssues.Any())
                return new Result(ShoppingErrors.CannotCheckoutWithValidationIssues);

            session.StartPurchase();
            _shoppingRepository.Save(session);

            var totalPrice = session.GetTotalPrice();
            var confirmationCode = _paymentService.CapturePayment(session.Id, session.PaymentToken, totalPrice);
            session.AttachPaymentConfirmation(confirmationCode);
            _shoppingRepository.Save(session);

            foreach (var seat in session.SelectedSeats)
            {
                _seatingRepository.MarkSeatPurchased(session.Id, session.EventId, seat.Id);
            }

            return new Result(null);
        }
    }
}

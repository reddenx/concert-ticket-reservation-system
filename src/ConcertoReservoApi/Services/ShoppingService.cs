﻿using ConcertoReservoApi.Core;
using ConcertoReservoApi.Infrastructure.DataRepositories;
using ConcertoReservoApi.Infrastructure.Dtos.Shopping;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConcertoReservoApi.Infrastructure.Dtos.Shopping.ShoppingSessionView;
using static ConcertoReservoApi.Services.IPaymentService;
using static ConcertoReservoApi.Infrastructure.DataRepositories.ISeatingRepository;
using static ConcertoReservoApi.Services.IShoppingService;
using ConcertoReservoApi.Infrastructure;

namespace ConcertoReservoApi.Services
{
    public interface IShoppingService
    {
        enum ShoppingErrors
        {
            NotFound,
            DuplicateSessionCreated,
            TechnicalError,
            SelectedSeatTaken,
            UnacceptablyBadInput,
            CannotCheckoutWithValidationIssues,
            PaymentCaptureFailed,
            ObservedPriceHasChanged
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
        /// attempts to capture payment and mark seats as sold, builds receipt artifact, and returns seat references for redemption
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<ReceiptView> AttemptPurchase(string id, decimal expectedTotalPrice);
    }

    public class ShoppingService : IShoppingService
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<ShoppingService> _logger;
        private readonly ITimeService _timeService;

        private readonly IShoppingRepository _shoppingRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly ISeatingRepository _seatingRepository;

        public ShoppingService(IShoppingRepository shoppingRepository, ILogger<ShoppingService> logger, IEventsRepository eventsRepository, ISeatingRepository seatingRepository, IPaymentService paymentService, ITimeService timeService)
        {
            _shoppingRepository = shoppingRepository;
            _logger = logger;
            _eventsRepository = eventsRepository;
            _seatingRepository = seatingRepository;
            _paymentService = paymentService;
            _timeService = timeService;
        }

        //would normally validate against duplicate creation with infrastructure inputs like the ip, browser, and other data
        public Result<ShoppingSessionView> StartShopping(string eventId)
        {
            //check if event is real and sellable
            var eventInfo = _eventsRepository.GetEvent(eventId);
            var now = _timeService.GetCurrentTime();
            if (eventInfo == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);
            if (eventInfo.OverrideTicketsPurchasable ?? eventInfo.TicketSalesStartDate > now)
            {
                _logger.Warning("shopping session attempt on unavailable event", eventId);
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);
            }

            var session = _shoppingRepository.CreateShoppingSession(eventId);
            if (session == null)
            {
                _logger.Error("failed to create shopping session for event", eventId);
                return new Result<ShoppingSessionView>(null, ShoppingErrors.TechnicalError);
            }

            return new Result<ShoppingSessionView>(FromCore(session, now));
        }

        public Result<ShoppingSessionView> GetSession(string id)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            var now = _timeService.GetCurrentTime();
            return new Result<ShoppingSessionView>(FromCore(session, now));
        }

        public Result<AvailableEventSeatsView> GetAvailableSeating(string id)
        {
            //improvement:
            //- addition of seating service that takes the context of a shopping session to produce tailored offerings

            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<AvailableEventSeatsView>(null, ShoppingErrors.NotFound);

            //ideally this would be a domain service of some sort that takes more into account from the shopper, like affiliation and what not, going with something simple like avaiable seating
            var availableSeating = _seatingRepository.GetAvailableEventSeating(session.EventId);

            var dto = AvailableEventSeatsView.FromCore(availableSeating);
            return new Result<AvailableEventSeatsView>(dto);
        }

        /// <summary>
        /// looks up selected seats, ensures they're all available, adds them to shopping session, reserves them, then saves session.
        /// if anything goes wrong it attempts to remove seats from session and clear any reserved seating.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public Result<ShoppingSessionView> SelectSeating(string id, SeatSelectionDto[] selection)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            //gather all info
            session.ClearSelectedSeats();
            foreach (var selectedSeat in selection)
            {
                var eventSeat = _seatingRepository.GetEventSeat(session.EventId, selectedSeat.SeatId);
                if (eventSeat.Status != SeatStatuses.Available)
                    return new Result<ShoppingSessionView>(null, ShoppingErrors.SelectedSeatTaken);

                var selectedAddons = eventSeat.AvailableAddons.Where(a => selectedSeat.AddonIds.Contains(a.AddonId)).ToArray();
                session.AddSelectedSeating(new SeatSelection(
                    eventSeat.SeatId,
                    eventSeat.Label,
                    selectedAddons.Select(a => new SelectedSeatAddon(a.AddonId, a.Label, a.Price)).ToArray(),
                    eventSeat.Price));
            }

            //make reservations
            var anyReservationFailed = false;
            foreach (var seat in session.SelectedSeats)
            {
                anyReservationFailed = anyReservationFailed || !_seatingRepository.ReserveSeat(session.EventId, session.Id, seat.SeatId);
            }

            //I don't like the early return pattern, but didn't want to duplicate unreservation block, could use a second pass here
            if (!anyReservationFailed)
            {
                var now = _timeService.GetCurrentTime();
                session.EnsureCountdownStarted(now);
                var saved = _shoppingRepository.Save(session); //if this fails we should also remove reservations

                if (saved)
                {
                    return new Result<ShoppingSessionView>(FromCore(session, now));
                }
            }

            _logger.Error("failed to reserve seats", $"sessionid:{session.Id}", $"seatIds:{string.Join(",", session.SelectedSeats.Select(s => s.SeatId))}");

            //this is somewhat unnecessary as the seat reservation should expire when the shopping session expires if it was left dangling, but it would ruin that seat for this and all other users as it's no longer attached to the session
            foreach (var seat in session.SelectedSeats)
            {
                _seatingRepository.RemoveReservation(session.EventId, session.Id, seat.SeatId);
            }
            return new Result<ShoppingSessionView>(null, ShoppingErrors.SelectedSeatTaken);
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

            var now = _timeService.GetCurrentTime();
            return new Result<ShoppingSessionView>(FromCore(session, now));
        }

        public Result<ShoppingSessionView> UpdatePaymentInformation(string id, string paymentTokenizationId)
        {
            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ShoppingSessionView>(null, ShoppingErrors.NotFound);

            var valid = _paymentService.ValidatePaymentToken(paymentTokenizationId);
            if (!valid)
            {
                _logger.Error("given payment token did not validate with vendor", $"sessionId:{id}", $"token:{paymentTokenizationId}");
                return new Result<ShoppingSessionView>(null, ShoppingErrors.UnacceptablyBadInput);
            }

            session.AttachPaymentToken(paymentTokenizationId);

            _shoppingRepository.Save(session);

            var now = _timeService.GetCurrentTime();
            return new Result<ShoppingSessionView>(FromCore(session, now));
        }

        public Result<ReceiptView> AttemptPurchase(string id, decimal expectedTotalPrice)
        {
            //improvement: chunk into commands that have state, idempotency in each destructive command (payment, state transitions, etc.)
            //improvement: if made asynchronous, separate auth/capture if supported by vendor
            //improvement: have another immutable object for purchasing, like an order, for now using expected price input validation as a faster alternative during prototyping

            var session = _shoppingRepository.Get(id);
            if (session == null)
                return new Result<ReceiptView>(null, ShoppingErrors.NotFound);

            //validate
            var now = _timeService.GetCurrentTime();
            var validationIssues = session.GetValidationIssues(now);
            if (validationIssues.Any())
            {
                _logger.Warning("attempted checkout with validation issues", $"sessionId:{id}", $"validations:{string.Join(",", validationIssues)}");
                return new Result<ReceiptView>(null, ShoppingErrors.CannotCheckoutWithValidationIssues);
            }

            var receiptLineItems = session.BuildReceipt();
            var totalPrice = receiptLineItems.Sum(r => r.amount);
            if (totalPrice != expectedTotalPrice)
            {
                _logger.Error("attempted checkout with a mismatching observed price", $"sessionId:{id}", $"expectedTotal:{expectedTotalPrice}", $"actualTotal:{totalPrice}");
                return new Result<ReceiptView>(null, ShoppingErrors.ObservedPriceHasChanged);
            }

            //change state to prevent reentry
            //improvements: handling if this fails or is in bad state, 
            session.PurchaseStarted();
            _shoppingRepository.Save(session);

            //calculate total price, validate against what user confirmed, capture payment, save to session
            //improvement:
            //- duplicate payment detection in payment service

            var captureResult = _paymentService.CapturePayment(session.Id, session.PaymentReference, totalPrice);
            if (captureResult.Success)
            {
                session.PaymentCaptureSucceeded(
                    captureResult.CaptureConfirmationCode,
                    captureResult.AmountCaptured,
                    receiptLineItems,
                    captureResult.CaptureDate.UtcDateTime);
                _shoppingRepository.Save(session);
            }
            else
            {
                _logger.Error("payment capture failed during checkout", $"sessionId:{id}");
                session.PaymentCaptureFailed();
                _shoppingRepository.Save(session);
                return new Result<ReceiptView>(null, ShoppingErrors.PaymentCaptureFailed);
            }

            //mark seats as purchased and get reference code
            foreach (var seat in session.SelectedSeats)
            {
                //this could be moved ahead of payment as this is internally reversable
                var soldSeat = _seatingRepository.MarkSeatPurchased(session.EventId, seat.SeatId, session.Id);
                session.AttachPurchasedSeating(soldSeat.SeatId, soldSeat.PurchaserReference);
            }

            return new Result<ReceiptView>(ReceiptView.FromCore(session));
        }
    }
}

using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Core;
using ConcertoReservoApi.Infrastructure.Dtos.Venues;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConcertoReservoApi.Infrastructure.DataRepositories.ISeatingRepository;

namespace ConcertoReservoApi.Infrastructure.DataRepositories
{
    //not at all the best mocking pattern but I'm in a hurry to get this demoable
    public class MOCK_ONE_REPOSITORY :
        IEventsRepository,
        ISeatingRepository,
        IShoppingRepository,
        IVenueRepository
    {
        private readonly Dictionary<string, EventData> _eventData_eventId = new Dictionary<string, EventData>();
        private readonly Dictionary<string, VenueData> _venues_venueId = new Dictionary<string, VenueData>();
        private readonly Dictionary<string, List<VenueSectionData>> _venueSections_venueId = new Dictionary<string, List<VenueSectionData>>();
        private readonly Dictionary<string, List<EventSectionData>> _eventSections_eventId = new Dictionary<string, List<EventSectionData>>();
        private readonly Dictionary<string, SeatPurchaseCodeData> _seatPurchases_seatId = new Dictionary<string, SeatPurchaseCodeData>();
        private readonly Dictionary<string, ShoppingSession> _shoppingSessions_sessionId = new Dictionary<string, ShoppingSession>();

        private string GenerateId()
        {
            return Guid.NewGuid().ToString("N").ToLower(); ;
        }

        #region events

        public EventData CreateEvent(AuthenticatedUser user, string title, string description, DateTimeOffset eventDate)
        {
            var data = new EventData(
                GenerateId(),
                title,
                description,
                EventDataPublishStates.Draft,
                eventDate,
                DateTimeOffset.MaxValue,
                null, null, null, null);
            _eventData_eventId[data.Id] = data;
            return data;
        }

        public EventData GetEvent(string eventId)
        {
            return _eventData_eventId.GetValueOrDefault(eventId, null);
        }

        public EventData[] SearchPublicEvents(DateTimeOffset searchStartDate, DateTimeOffset searchEndDate)
        {
            return _eventData_eventId.Values.Where(e =>
                    e.EventDate > searchStartDate
                    && e.EventDate < searchEndDate
                    && e.PublishState == EventDataPublishStates.Published)
                .ToArray();
        }

        public EventData UpdateEvent(AuthenticatedUser user, EventData data)
        {
            _eventData_eventId[data.Id] = data;
            return data;
        }

        #endregion events

        #region venues

        public VenueData Get(string venueId)
        {
            return _venues_venueId.GetValueOrDefault(venueId, null);
        }

        public Dictionary<string, VenueData> GetVenues(string[] venueIds)
        {
            return _venues_venueId.Where(kvp => venueIds.Contains(kvp.Key)).ToDictionary();
        }

        public VenueData[] GetAllVenues()
        {
            return _venues_venueId.Values.ToArray();
        }

        public VenueData Create(AuthenticatedUser user, string name, string description)
        {
            var venue = new VenueData(GenerateId(), name, description);
            _venues_venueId[venue.Id] = venue;
            _venueSections_venueId[venue.Id] = new List<VenueSectionData>();
            return venue;
        }

        public VenueData UpdateVenue(AuthenticatedUser user, string venueId, string name, string description)
        {
            var venue = new VenueData(venueId, name, description);
            _venues_venueId[venue.Id] = venue;
            return venue;
        }

        public void DeleteAllSections(AuthenticatedUser user, string venueId)
        {
            _venueSections_venueId[venueId]?.Clear();
        }

        public void AddSection(AuthenticatedUser user, string venueId, VenueSectionData section)
        {
            _venueSections_venueId[venueId]?.Add(section);
        }

        #endregion venues

        #region seating

        public ISeatingRepository.EventSeatData GetEventSeat(string eventId, string seatId)
        {
            if (!_eventSections_eventId.ContainsKey(eventId))
                return null;

            foreach (var section in _eventSections_eventId[eventId])
                return section.Seats.FirstOrDefault(s => s.SeatId == seatId);
            return null;
        }

        public void DeleteEventSeating(string eventId)
        {
            _eventSections_eventId.Remove(eventId);
        }

        public void CreateSeatingForEvent(string eventId, string venueId, EventSectionConfigurationData[] sectionConfiguration)
        {
            var createdSections = new List<EventSectionData>();
            foreach (var section in sectionConfiguration)
            {
                var venueSection = _venueSections_venueId[venueId].FirstOrDefault(s => s.SectionId == section.SectionId);

                var seats = venueSection.Seats.Select(s => new EventSeatData(
                        s.SeatId,
                        SeatStatuses.Available,
                        s.Label,
                        [],
                        section.BasePrice,
                        s.DisplayPosition))
                    .ToArray();

                createdSections.Add(new EventSectionData(
                    section.SectionId,
                    venueSection.Label,
                    seats,
                    venueSection.DisplayPolygon,
                    venueSection.DisplayPosition));
            }
            _eventSections_eventId[eventId] = createdSections;
        }

        public EventSectionData[] GetAvailableEventSeating(string eventId)
        {
            return _eventSections_eventId[eventId].ToArray();
        }

        public bool ReserveSeat(string eventId, string shoppingSessionId, string seatId)
        {
            var foundSection = _eventSections_eventId[eventId].FirstOrDefault(sect => sect.Seats.Any(s => s.SeatId == seatId));
            if (foundSection == null)
                return false;

            var foundSeat = foundSection.Seats.First(s => s.SeatId == seatId);
            if (foundSeat.Status != SeatStatuses.Available)
                return false;

            var newSeats = foundSection.Seats
                .Where(s => s.SeatId != seatId)
                .Concat([new EventSeatData(
                    foundSeat.SeatId,
                    SeatStatuses.Reserved,
                    foundSeat.Label,
                    foundSeat.AvailableAddons,
                    foundSeat.Price,
                    foundSeat.DisplayPosition)]);
            var newSection = new EventSectionData(
                foundSection.SectionId,
                foundSection.Label,
                newSeats.ToArray(),
                foundSection.DisplayPolygon,
                foundSection.DisplayPosition);

            _eventSections_eventId[eventId].Remove(foundSection);
            _eventSections_eventId[eventId].Add(newSection);
            return true;
        }

        public void RemoveReservation(string eventId, string shoppingSessionId, string seatId)
        {
            var foundSection = _eventSections_eventId[eventId].FirstOrDefault(sect => sect.Seats.Any(s => s.SeatId == seatId));
            if (foundSection == null)
                return;

            var foundSeat = foundSection.Seats.First(s => s.SeatId == seatId);
            if (foundSeat.Status != SeatStatuses.Reserved)
                return;

            var newSeats = foundSection.Seats
                .Where(s => s.SeatId != seatId)
                .Concat([new EventSeatData(
                    foundSeat.SeatId,
                    SeatStatuses.Available,
                    foundSeat.Label,
                    foundSeat.AvailableAddons,
                    foundSeat.Price,
                    foundSeat.DisplayPosition)]);
            var newSection = new EventSectionData(
                foundSection.SectionId,
                foundSection.Label,
                newSeats.ToArray(),
                foundSection.DisplayPolygon,
                foundSection.DisplayPosition);

            _eventSections_eventId[eventId].Remove(foundSection);
            _eventSections_eventId[eventId].Add(newSection);
        }

        public ISeatingRepository.SeatPurchaseCodeData MarkSeatPurchased(string eventId, string seatId, string shoppingSessionId)
        {
            var foundSection = _eventSections_eventId[eventId].FirstOrDefault(sect => sect.Seats.Any(s => s.SeatId == seatId));
            if (foundSection == null)
                return null;

            var foundSeat = foundSection.Seats.First(s => s.SeatId == seatId);

            var newSeats = foundSection.Seats
                .Where(s => s.SeatId != seatId)
                .Concat([new EventSeatData(
                    foundSeat.SeatId,
                    SeatStatuses.Purchased,
                    foundSeat.Label,
                    foundSeat.AvailableAddons,
                    foundSeat.Price,
                    foundSeat.DisplayPosition)]);
            var newSection = new EventSectionData(
                foundSection.SectionId,
                foundSection.Label,
                newSeats.ToArray(),
                foundSection.DisplayPolygon,
                foundSection.DisplayPosition);

            _eventSections_eventId[eventId].Remove(foundSection);
            _eventSections_eventId[eventId].Add(newSection);

            var purchaseCode = new SeatPurchaseCodeData(foundSeat.SeatId, GenerateId(), shoppingSessionId);
            _seatPurchases_seatId[foundSeat.SeatId] = purchaseCode;

            return purchaseCode;
        }

        public bool HasEventHadAnySalesActivity(string eventId)
        {
            return _eventSections_eventId[eventId].SelectMany(s => s.Seats).Any(s => s.Status != SeatStatuses.Available);
        }

        public SeatPurchaseCodeData[] GetReferenceCodesForPurchase(string shoppingSessionId)
        {
            return _seatPurchases_seatId.Values.Where(s => s.ShoppingSessionId == shoppingSessionId).ToArray();
        }

        #endregion seating

        #region shopping

        public ShoppingSession CreateShoppingSession(string eventId)
        {
            var newSession = new ShoppingSession(
                GenerateId(),
                eventId);

            _shoppingSessions_sessionId.Add(newSession.Id, newSession);

            return newSession;
        }

        ShoppingSession IShoppingRepository.Get(string id)
        {
            return _shoppingSessions_sessionId[id];
        }

        public bool Save(ShoppingSession session)
        {
            session.RecordSaved();
            _shoppingSessions_sessionId[session.Id] = session;
            return true;
        }

        #endregion shopping
    }
}

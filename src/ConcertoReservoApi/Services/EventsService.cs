using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Infrastructure;
using ConcertoReservoApi.Infrastructure.Dtos.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConcertoReservoApi.Services.IEventsService;

namespace ConcertoReservoApi.Services
{
    public interface IEventsService
    {
        enum EventServiceErrors
        {
            NotFound, BadInput,
            CannotModifyEventIfSeatsHaveSold
        }
        record Result<T>(T Data, EventServiceErrors? Error)
        {
            public bool HasErrors => Error.HasValue;
        }

        PublicEventView GetPublicEvent(string id);
        PublicEventView[] GetUpcomingPublicEvents();

        Result<EventDto> CreateEvent(AuthenticatedUser user, EventDto dto);
        Result<EventDto> UpdateEvent(AuthenticatedUser user, EventDto dto);
    }

    public class EventsService : IEventsService
    {
        private readonly ITimeService _timeService;

        private readonly IEventsRepository _eventsRepository;
        private readonly ISeatingRepository _seatingRepository;
        private readonly IVenueRepository _venueRepository;

        public EventsService(IEventsRepository eventsRepository, ITimeService timeService, ISeatingRepository seatingRepository, IVenueRepository venueRepository)
        {
            _eventsRepository = eventsRepository;
            _timeService = timeService;
            _seatingRepository = seatingRepository;
            _venueRepository = venueRepository;
        }

        public PublicEventView[] GetUpcomingPublicEvents()
        {
            var now = _timeService.GetCurrentTime();
            var upcomingEvents = _eventsRepository.SearchPublicEvents(now.AddDays(-3), now.AddMonths(6));
            var venues = _venueRepository.GetVenues(upcomingEvents.Select(v => v.VenueId).Distinct().ToArray());

            return upcomingEvents
                .Select(e => PublicEventView.FromData(e, now, venues.GetValueOrDefault(e.VenueId, null)))
                .ToArray();
        }

        public PublicEventView GetPublicEvent(string id)
        {
            var @event = _eventsRepository.GetEvent(id);
            var now = _timeService.GetCurrentTime();

            if (@event == null
                || @event.PublishState != EventDataPublishStates.Published)
                return null;

            var venue = _venueRepository.Get(@event.VenueId);

            return PublicEventView.FromData(@event, now, venue);
        }

        public Result<EventDto> CreateEvent(AuthenticatedUser user, EventDto dto)
        {
            var eventData = _eventsRepository.CreateEvent(user,
                dto.Title,
                dto.Description,
                _timeService.FromUtcInput(dto.EventDate));

            dto.Id = eventData.Id;
            return UpdateEvent(user, dto);
        }

        public Result<EventDto> UpdateEvent(AuthenticatedUser user, EventDto dto)
        {
            var eventData = _eventsRepository.GetEvent(dto.Id);
            if (eventData == null)
                return new Result<EventDto>(null, EventServiceErrors.NotFound);

            return UpdateEvent(user, eventData, dto);
        }

        private Result<EventDto> UpdateEvent(AuthenticatedUser user, EventData eventData, EventDto dto)
        {
            //some things to prevent:
            //- modifying event seating if seats have any sales activity
            if (_seatingRepository.HasEventHadAnySalesActivity(eventData.Id))
                return new Result<EventDto>(null, EventServiceErrors.CannotModifyEventIfSeatsHaveSold);

            var updatedEvent = new EventData(
                eventData.Id,
                dto.Title,
                dto.Description,
                EventDto.Map(dto.PublishState),
                _timeService.FromUtcInput(dto.EventDate),
                _timeService.FromUtcInput(dto.TicketSaleStartDate),
                dto.OverrideTicketsShoppable,
                dto.OverrideTicketsPurchasable,
                dto.VenueConfiguration?.VenueId,
                dto.VenueConfiguration?.SelectedSectionIds);

            _eventsRepository.UpdateEvent(updatedEvent);

            //regenerate seating info
            if (dto.PublishState == EventDto.EventPublishStates.Published
                && dto.VenueConfiguration?.VenueId != null
                && dto.VenueConfiguration?.SelectedSectionIds?.Any() == true)
            {
                _seatingRepository.DeleteEventSeating(eventData.Id);
                _seatingRepository.CreateSeatingForEvent(eventData.Id, eventData.VenueId, eventData.SelectedEventSectionIds);
            }

            return new Result<EventDto>(EventDto.FromData(updatedEvent), null);
        }
    }
}

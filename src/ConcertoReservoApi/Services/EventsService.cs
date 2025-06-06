using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Infrastructure.Dtos.Events;
using System;
using static ConcertoReservoApi.Services.IEventsService;

namespace ConcertoReservoApi.Services
{
    public interface IEventsService
    {
        enum EventServiceErrors { NotFound, BadInput }
        record Result<T>(T Data, EventServiceErrors? Error)
        {
            public bool HasErrors => Error.HasValue;
        }

        PublicEventView GetPublicEvent(string id);
        PublicEventView GetUpcomingPublicEvents();

        Result<EventDto> CreateEvent(AuthenticatedUser user, EventDto dto);
        Result<EventDto> UpdateEvent(AuthenticatedUser user, EventDto dto);
    }

    public class EventsService : IEventsService
    {
        public Result<EventDto> CreateEvent(AuthenticatedUser user, EventDto dto)
        {
            throw new NotImplementedException();
        }

        public PublicEventView GetPublicEvent(string id)
        {
            throw new NotImplementedException();
        }

        public PublicEventView GetUpcomingPublicEvents()
        {
            throw new NotImplementedException();
        }

        public Result<EventDto> UpdateEvent(AuthenticatedUser user, EventDto dto)
        {
            throw new NotImplementedException();
        }
    }
}

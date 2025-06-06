using ConcertoReservoApi.Controllers;
using static ConcertoReservoApi.Controllers.EventsController;

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
        public IEventsService.Result<EventDto> CreateEvent(AuthenticatedUser user, EventDto dto)
        {
            throw new System.NotImplementedException();
        }

        public PublicEventView GetPublicEvent(string id)
        {
            throw new System.NotImplementedException();
        }

        public PublicEventView GetUpcomingPublicEvents()
        {
            throw new System.NotImplementedException();
        }

        public IEventsService.Result<EventDto> UpdateEvent(AuthenticatedUser user, EventDto dto)
        {
            throw new System.NotImplementedException();
        }
    }
}

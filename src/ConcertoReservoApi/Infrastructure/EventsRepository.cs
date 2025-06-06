using ConcertoReservoApi.Core;
using Microsoft.Extensions.Logging;

namespace ConcertoReservoApi.Infrastructure
{
    public interface IEventsRepository
    {
        record Event(string id);

        Event GetEvent(string eventId);
    }
}

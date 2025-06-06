using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Infrastructure
{
    public interface IEventsRepository
    {
        object[] GetAvailableEventSeating(string eventId);
        Event GetEvent(string eventId);
    }
    public class MockEventsRepository : IEventsRepository { }
}

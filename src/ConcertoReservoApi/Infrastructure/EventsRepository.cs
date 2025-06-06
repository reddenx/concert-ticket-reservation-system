using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Infrastructure
{
    public interface IEventsRepository
    {
        object[] GetAvailableEventSeating(string eventId);
        Event GetEvent(string eventId);
    }
    public class MockEventsRepository : IEventsRepository
    {
        public object[] GetAvailableEventSeating(string eventId)
        {
            throw new System.NotImplementedException();
        }

        public Event GetEvent(string eventId)
        {
            throw new System.NotImplementedException();
        }
    }
}

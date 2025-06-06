using System;

namespace ConcertoReservoApi.Services
{
    public interface ITimeService
    {
        DateTimeOffset GetCurrentTime();
    }
    public class TimeService : ITimeService
    {
        public DateTimeOffset GetCurrentTime()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}

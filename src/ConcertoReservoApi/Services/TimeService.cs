using System;

namespace ConcertoReservoApi.Services
{
    public interface ITimeService
    {
        DateTimeOffset FromUtcInput(DateTime utcLocalTime);
        DateTimeOffset GetCurrentTime();
    }
    public class LocalUtcTimeService : ITimeService
    {
        //purpose, central place to correct time issues from hosting environment
        public DateTimeOffset FromUtcInput(DateTime utcLocalTime)
        {
            return new DateTimeOffset(utcLocalTime, TimeSpan.Zero);
        }

        public DateTimeOffset GetCurrentTime()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}

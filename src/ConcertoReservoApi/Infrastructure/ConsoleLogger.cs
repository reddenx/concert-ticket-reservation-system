using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ConcertoReservoApi.Infrastructure
{
    public interface ILogger<T>
    {
        void Warning(string message, params string[] extraData);
        void Error(string message, params string[] extraData);
        void Error(string message, Exception ex, params string[] extraData);
    }

    public class ConsoleLogger<T> : ILogger<T>
    {
        public void Info(string message, params string[] extraData)
            => Write("INFO", message, extraData);   

        public void Warning(string message, params string[] extraData)
            => Write("WARNING", message, extraData);

        public void Error(string message, params string[] extraData)
            => Write("ERROR", message, extraData);

        public void Error(string message, Exception ex, params string[] extraData)
            => Write("Error", message, extraData.Concat([ex.ToString()]).ToArray());

        private void Write(string level, string message, params string[] extraData)
        {
            Console.WriteLine($"{DateTime.UtcNow:s} [{level}]: {typeof(T)} {message} {string.Join(",", extraData)}");
        }
    }
}

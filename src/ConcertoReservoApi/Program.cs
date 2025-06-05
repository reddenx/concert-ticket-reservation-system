using Microsoft.AspNetCore.Builder;

namespace ConcertoReservoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //TODO
            //- security
            //- hosting parameters
            //- configuration

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.Run();
        }
    }
}

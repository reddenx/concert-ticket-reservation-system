using ConcertoReservoApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

            builder.Services.AddControllers();
            DependencyConfiguration.Register(builder);

            builder.Services.AddAuthentication();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}

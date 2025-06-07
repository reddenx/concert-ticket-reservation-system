using ConcertoReservoApi.Infrastructure.DataRepositories;
using ConcertoReservoApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConcertoReservoApi.Infrastructure
{
    public static class DependencyConfiguration
    {
        public static void Register(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            builder.Services.AddSingleton<IShoppingService, ShoppingService>();
            builder.Services.AddSingleton<IEventsService, EventsService>();
            builder.Services.AddSingleton<IVenueService, VenueService>();
            builder.Services.AddSingleton<ITimeService, LocalUtcTimeService>();
            builder.Services.AddSingleton<IPaymentService, MOCK_PAYMENT_SERVICE>();

            builder.Services.AddSingleton<MOCK_ONE_REPOSITORY>();
            builder.Services.AddSingleton<IEventsRepository>(p => p.GetService<MOCK_ONE_REPOSITORY>());
            builder.Services.AddSingleton<ISeatingRepository>(p => p.GetService<MOCK_ONE_REPOSITORY>());
            builder.Services.AddSingleton<IShoppingRepository>(p => p.GetService<MOCK_ONE_REPOSITORY>());
            builder.Services.AddSingleton<IVenueRepository>(p => p.GetService<MOCK_ONE_REPOSITORY>());

            // improvement: I'd like to use the baked in microsoft logging package but it's changed a lot since I've last used it
            // and didn't have time familiarize myself with the modern patterns, here's an unfortunate half-measure
            builder.Services.AddSingleton<ILogger<ShoppingService>, ConsoleLogger<ShoppingService>>();
        }
    }
}

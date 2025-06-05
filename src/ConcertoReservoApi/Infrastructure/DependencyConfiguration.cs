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
            builder.Services.AddSingleton<IShoppingService, ShoppingService>();
            builder.Services.AddSingleton<IShoppingRepository, MockShoppingRepository>();

            builder.Services.AddSingleton<IEventsService, EventsService>();
            builder.Services.AddSingleton<IEventsRepository, MockEventsRepository>();

            builder.Services.AddSingleton<IVenueService, VenueService>();
            builder.Services.AddSingleton<IVenueRepository, MockVenueService>();
        }
    }
}

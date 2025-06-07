using System;
using ConcertoReservoApi.Infrastructure.DataRepositories;

namespace ConcertoReservoApi.Infrastructure.Dtos.Events;

public class PublicEventView
{
    public string EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string VenueName { get; set; }
    public string VenueId { get; set; }

    public bool CanShopForTickets { get; set; }
    public bool CanPurchaseTickets { get; set; }

    public DateTime TicketSaleStartDate { get; set; }
    public DateTime EventDate { get; set; }

    public static PublicEventView FromData(EventData @event, DateTimeOffset now, VenueData venue)
    {
        return new PublicEventView
        {
            EventId = @event.Id,
            Title = @event.Title,
            Description = @event.Description,
            VenueName = venue.Name,
            VenueId = @event.VenueId,
            CanShopForTickets = @event.OverrideTicketsShoppable ?? @event.TicketSalesStartDate > now,
            CanPurchaseTickets = @event.OverrideTicketsPurchasable ?? @event.TicketSalesStartDate > now,
            TicketSaleStartDate = @event.TicketSalesStartDate.UtcDateTime,
            EventDate = @event.EventDate.UtcDateTime,
        };
    }
}

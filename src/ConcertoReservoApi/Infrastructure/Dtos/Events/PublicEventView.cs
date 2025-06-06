using System;

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
}

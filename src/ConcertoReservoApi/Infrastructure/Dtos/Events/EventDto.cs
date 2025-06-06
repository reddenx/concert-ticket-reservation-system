using System;

namespace ConcertoReservoApi.Infrastructure.Dtos.Events;

public class EventDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public EventPublishStates PublishState { get; set; }

    public DateTime TicketSaleStartDate { get; set; }
    public DateTime EventDate { get; set; }

    //configured separately to allow the soft closing of ticket sales while people are in the shopping process
    public bool? OverrideTicketsShoppable { get; set; } //allows users to start a shopping session/users to enter seating selection (pauses queue)
    public bool? OverrideTicketsPurchasable { get; set; } //allows users to purchase seats

    public VenueConfigurationDto VenueConfiguration { get; set; }

    public enum EventPublishStates { Draft, Published }
    public class VenueConfigurationDto
    {
        public string[] SelectedSectionIds { get; set; }
    }
}

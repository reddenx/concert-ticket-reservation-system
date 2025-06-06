using System;
using System.Linq;

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

    public enum EventPublishStates { Draft, Deleted, Published }
    public class VenueConfigurationDto
    {
        public string VenueId { get; set; }
        public VenueSectionConfigurationDto[] SectionConfigurations { get; set; }
    }
    public class VenueSectionConfigurationDto
    {
        public string SectionId { get; set; }
        public decimal SeatPrice { get; set; }
    }

    public static EventDto FromData(EventData eventData)
    {
        return new EventDto
        {
            Id = eventData.Id,
            Title = eventData.Title,
            Description = eventData.Description,
            PublishState = Map(eventData.PublishState),
            TicketSaleStartDate = eventData.TicketSalesStartDate.UtcDateTime,
            EventDate = eventData.EventDate.UtcDateTime,
            OverrideTicketsPurchasable = eventData.OverrideTicketsPurchasable,
            OverrideTicketsShoppable = eventData.OverrideTicketsShoppable,
            VenueConfiguration = new VenueConfigurationDto()
            {
                VenueId = eventData.VenueId,
                SectionConfigurations = eventData.SectionConfigurations.Select(c => new VenueSectionConfigurationDto
                {
                    SeatPrice = c.BasePrice,
                    SectionId = c.SectionId
                }).ToArray(),
            }
        };
    }
    public static EventPublishStates Map(EventDataPublishStates publishState) => publishState switch
    {
        EventDataPublishStates.Draft => EventPublishStates.Draft,
        EventDataPublishStates.Published => EventPublishStates.Published,
        EventDataPublishStates.Deleted => EventPublishStates.Deleted,
        _ => throw new ArgumentOutOfRangeException()
    };
    public static EventDataPublishStates Map(EventPublishStates state) => state switch
    {
        EventPublishStates.Draft => EventDataPublishStates.Draft,
        EventPublishStates.Published => EventDataPublishStates.Published,
        EventPublishStates.Deleted => EventDataPublishStates.Deleted,
        _ => throw new ArgumentOutOfRangeException()
    };
}

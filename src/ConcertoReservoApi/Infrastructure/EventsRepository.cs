using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Core;
using Microsoft.Extensions.Logging;
using System;

namespace ConcertoReservoApi.Infrastructure
{
    public enum EventDataPublishStates { Draft, Deleted, Published }
    public record EventData(
        string Id,
        string Title,
        string Description,
        EventDataPublishStates PublishState,
        DateTimeOffset EventDate,
        DateTimeOffset TicketSalesStartDate,
        bool? OverrideTicketsShoppable,
        bool? OverrideTicketsPurchasable,
        string VenueId,
        EventSectionData[] SectionConfigurations);
    public record EventSectionData(string SectionId, decimal BasePrice);

    public interface IEventsRepository
    {
        EventData[] SearchPublicEvents(DateTimeOffset searchStartDate, DateTimeOffset searchEndDate);
        EventData GetEvent(string eventId);
        EventData CreateEvent(AuthenticatedUser user, string title, string description, DateTimeOffset eventDate);
        EventData UpdateEvent(params object[] inputs);
    }
}

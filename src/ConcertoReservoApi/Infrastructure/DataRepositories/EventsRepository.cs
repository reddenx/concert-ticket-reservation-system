using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Core;
using Microsoft.Extensions.Logging;
using System;

namespace ConcertoReservoApi.Infrastructure.DataRepositories
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
        EventSectionConfigurationData[] SectionConfigurations);
    public record EventSectionConfigurationData(string SectionId, decimal BasePrice);

    public interface IEventsRepository
    {
        EventData[] SearchPublicEvents(DateTimeOffset searchStartDate, DateTimeOffset searchEndDate);
        EventData GetEvent(string eventId);
        EventData CreateEvent(AuthenticatedUser user, string title, string description, DateTimeOffset eventDate);
        EventData UpdateEvent(AuthenticatedUser user, EventData data);
    }
}

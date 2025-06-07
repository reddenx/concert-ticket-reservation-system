using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Core;
using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;

namespace ConcertoReservoApi.Infrastructure.DataRepositories;

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

//example of how I like to do db repositories
public class EventsRepository : IEventsRepository
{
    public interface IEventsRepositoryConfiguration { string ConnectionString { get; } }

    private readonly IEventsRepositoryConfiguration _config;

    public EventData CreateEvent(AuthenticatedUser user, string title, string description, DateTimeOffset eventDate)
    {
        using (var conn = new MySqlConnection(_config.ConnectionString))
        {
            //just an example
            var eventId = Guid.NewGuid().ToString("N").ToString();
            conn.Execute(@"insert into events values (@id, @title, @description, @eventDate);",
                new
                {
                    id = eventId,
                    title = title,
                    description = description,
                    eventDate = eventDate.ToUniversalTime()
                });
            return GetEvent(conn, eventId);
        }
    }

    public EventData GetEvent(string eventId)
    {
        throw new NotImplementedException();
    }

    private EventData GetEvent(MySqlConnection conn, string eventId) 
    {
        throw new NotImplementedException();
    }

    public EventData[] SearchPublicEvents(DateTimeOffset searchStartDate, DateTimeOffset searchEndDate)
    {
        throw new NotImplementedException();
    }

    public EventData UpdateEvent(AuthenticatedUser user, EventData data)
    {
        throw new NotImplementedException();
    }
}
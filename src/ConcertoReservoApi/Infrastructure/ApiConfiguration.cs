using Microsoft.Extensions.Configuration;
using static ConcertoReservoApi.Infrastructure.DataRepositories.EventsRepository;

namespace ConcertoReservoApi.Infrastructure;

public class ApiConfiguration
    : IEventsRepositoryConfiguration
{
    private readonly IConfiguration _config;

    public ApiConfiguration(IConfiguration config)
    {
        _config = config;
    }

    public string ConnectionString => _config.GetConnectionString("tickets-api");
}

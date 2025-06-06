using System.Collections.Generic;

namespace ConcertoReservoApi.Infrastructure
{
    public record VenueData(string Id, string Name);

    public interface IVenueRepository
    {
        VenueData Get(string venueId);
        Dictionary<string, VenueData> GetVenues(string[] venueIds);
    }
    //public class MockVenueService : IVenueRepository { }
}

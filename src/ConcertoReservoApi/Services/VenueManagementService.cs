using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Services
{
    public interface IVenueManagementService
    {
        record Result<T>;
        record VenueDto;

        Result<Venue> CreateVenue(params object[] inputs);
        Result<Venue> UpdateVenue(VenueDto venue);
        Result<Venue> GetVenue(Id<Venue> venueId);
    }
}

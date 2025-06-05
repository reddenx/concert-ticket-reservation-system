using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Services
{
    public interface IVenuManagementService
    {
        record Result<T>;

        Result<Venue> CreateVenue(params object[] inputs);

    }
}

using ConcertoReservoApi.Controllers;
using static ConcertoReservoApi.Controllers.VenueController;

namespace ConcertoReservoApi.Services
{
    public interface IVenueService
    {
        PublicVenueListView[] GetAllVenuesForCms();
        PublicVenueListView[] GetPublicVenues();

        VenueDto GetVenue(string id);
        VenueDto CreateVenue(AuthenticatedUser user, VenueController.VenueDto dto);
        bool UpdateVenue(AuthenticatedUser user, string id, VenueController.VenueDto dto);
    }
    public class VenueService : IVenueService
    {
    }
}

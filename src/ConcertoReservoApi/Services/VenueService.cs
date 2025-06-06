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
        public VenueDto CreateVenue(AuthenticatedUser user, VenueDto dto)
        {
            throw new System.NotImplementedException();
        }

        public PublicVenueListView[] GetAllVenuesForCms()
        {
            throw new System.NotImplementedException();
        }

        public PublicVenueListView[] GetPublicVenues()
        {
            throw new System.NotImplementedException();
        }

        public VenueDto GetVenue(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateVenue(AuthenticatedUser user, string id, VenueDto dto)
        {
            throw new System.NotImplementedException();
        }
    }
}

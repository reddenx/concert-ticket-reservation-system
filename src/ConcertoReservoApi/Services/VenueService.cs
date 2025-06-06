using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Infrastructure;
using ConcertoReservoApi.Infrastructure.Dtos.Venues;
using System;
using System.Linq;

namespace ConcertoReservoApi.Services
{
    //this service was done in a hurry and is not production ready in the slightest

    public interface IVenueService
    {
        PublicVenueListView[] GetAllVenuesForCms();
        PublicVenueListView[] GetPublicVenues();

        VenueDto GetVenue(string venueId);
        VenueDto CreateVenue(AuthenticatedUser user, VenueDto dto);
        VenueDto UpdateVenue(AuthenticatedUser user, VenueDto dto);
        bool UpdateVenueSections(AuthenticatedUser user, string venueId, VenueSectionDto[] dtos);
    }
    public class VenueService : IVenueService
    {
        private IVenueRepository _venueRepository;

        public VenueService(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }
        public PublicVenueListView[] GetAllVenuesForCms()
        {
            //ideally there'd be an info publishing status management system for venues but time constraints strike again
            //for now they're all gonna look pretty identical
            var venues = _venueRepository.GetAllVenues();
            return venues.Select(PublicVenueListView.FromData).ToArray();
        }
        public PublicVenueListView[] GetPublicVenues()
        {
            //see note from cms getter
            var venues = _venueRepository.GetAllVenues();
            return venues.Select(PublicVenueListView.FromData).ToArray();
        }
        public VenueDto GetVenue(string venueId)
        {
            var venue = _venueRepository.Get(venueId);
            if (venue == null)
                return null;

            return VenueDto.FromData(venue);
        }
        public VenueDto CreateVenue(AuthenticatedUser user, VenueDto dto)
        {
            var newVenue = _venueRepository.Create(user, dto.Name, dto.Description);
            return VenueDto.FromData(newVenue);
        }
        public VenueDto UpdateVenue(AuthenticatedUser user, VenueDto dto)
        {
            var updatedVenue = _venueRepository.UpdateVenue(user, dto.Id, dto.Name, dto.Description);
            return VenueDto.FromData(updatedVenue);
        }
        public bool UpdateVenueSections(AuthenticatedUser user, string venueId, VenueSectionDto[] dtos)
        {
            //there would ideally be much more validation around this, as well as some sort of timed versioning system that allowed
            //for the updating of configurations at a given point in time as to not interrupt already set up events, however 
            //time constraints has limited me to a naive overwriting implementation :/

            _venueRepository.DeleteAllSections(user, venueId);
            foreach (var section in dtos)
            {
                _venueRepository.AddSection(user, venueId, section);
            }
            return true;
        }
    }
}

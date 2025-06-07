using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Infrastructure.Dtos.Venues;
using System.Collections.Generic;
using System.Drawing;

namespace ConcertoReservoApi.Infrastructure.DataRepositories
{
    public record VenueData(string Id, string Name, string Description);
    public record VenueSectionData(string VenueId, string SectionId, string Label, string Description, Point[] DisplayPolygon, Point DisplayPosition, VenueSeatingData[] Seats);
    public record VenueSeatingData(string VenueId, string SectionId, string SeatId, string Label, string Description, Point DisplayPosition);

    public interface IVenueRepository
    {
        VenueData Get(string venueId);
        Dictionary<string, VenueData> GetVenues(string[] venueIds);
        VenueData[] GetAllVenues();
        VenueData Create(AuthenticatedUser user, string name, string description);
        VenueData UpdateVenue(AuthenticatedUser user, string venueId, string name, string description);

        void DeleteAllSections(AuthenticatedUser user, string venueId);
        void AddSection(AuthenticatedUser user, string venueId, VenueSectionData section);
    }
}

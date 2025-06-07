using System;
using ConcertoReservoApi.Infrastructure.DataRepositories;

namespace ConcertoReservoApi.Infrastructure.Dtos.Venues;

public class VenueDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    internal static VenueDto FromData(VenueData venue)
    {
        return new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Description = venue.Description,
        };
    }

    //probably needs some sort of versioning system necessary for venue changes, like can't get rid of the ice rink if there's a hockey game on the calendar sort of thing
}

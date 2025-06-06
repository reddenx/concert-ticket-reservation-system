using System;
using System.Collections.Generic;

namespace ConcertoReservoApi.Infrastructure.Dtos.Venues;

public class PublicVenueListView
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public static PublicVenueListView FromData(VenueData venue)
    {
        return new PublicVenueListView
        {
            Id = venue.Id,
            Name = venue.Name,
            Description = venue.Description,
        };
    }
}

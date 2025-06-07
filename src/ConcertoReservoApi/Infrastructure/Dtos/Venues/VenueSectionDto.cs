using ConcertoReservoApi.Infrastructure.DataRepositories;
using System;
using System.Drawing;
using System.Linq;

namespace ConcertoReservoApi.Infrastructure.Dtos.Venues;

public class VenueSectionDto
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Description { get; set; }
    public VenueSeatDto[] Seats { get; set; }

    //display
    public Point[] DisplayPolygon { get; set; }
    public Point DisplayPosition { get; set; }

    internal static VenueSectionDto FromData(VenueSectionData data)
    {
        return new VenueSectionDto
        {
            Description = data.Description,
            DisplayPolygon = data.DisplayPolygon,
            DisplayPosition = data.DisplayPosition,
            Id = data.SectionId,
            Label = data.Label,
            Seats = data.Seats.Select(s => new VenueSeatDto
            {
                Id = s.SeatId,
                Label = s.Label,
                Description = s.Description,
            }).ToArray(),
        };
    }

    //would probably want some mutual exclusivity with other sections, e.g. can't have a concert pit and floor seating at the same event. the user is assumed to be expert for this.
    public class VenueSeatDto
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        //public int Capacity { get; set; } //for things like a floor/pit area, supports tables at comedy clubs, benches at certain venues, etc.

        //display
        public Point Position;
    }
}
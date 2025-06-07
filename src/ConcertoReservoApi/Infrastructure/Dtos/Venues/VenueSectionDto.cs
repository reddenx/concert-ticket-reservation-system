using System.Drawing;

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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;

namespace ConcertoReservoApi.Controllers
{
    [Route("venues")]
    public class VenueController : Controller
    {
        public class PublicVenueListView
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class VenueDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            
            //probably needs some sort of versioning system necessary for venue changes, like can't get rid of the ice rink if there's a hockey game on the calendar sort of thing
        }

        public class VenueSectionDto
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public string Description { get; set; }
            public VenueSeatDto[] Seats { get; set; }

            //display
            public Point[] DisplayPolygon { get; set; }
            public Point SeatOrigin { get; set; }

            //would probably want some mutual exclusivity with other sections, e.g. can't have a concert pit and floor seating at the same event. the user is assumed to be expert for this.
        }

        public class VenueSeatDto
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public string Description { get; set; }
            public int Capacity { get; set; } //for things like a floor/pit area, supports tables at comedy clubs, benches at certain venues, etc.
            
            //display
            public Point Position;
        }

        //powers public facing venue selection UIs
        [HttpGet("public")]
        [Produces<PublicVenueListView[]>]
        [AllowAnonymous]
        public IActionResult GetAllPublicVenues()
            => throw new NotImplementedException();

        [HttpGet("public")]
        [Produces<PublicVenueListView[]>]
        public IActionResult GetAllVenues()
            => throw new NotImplementedException();

        //
        [HttpPost("")]
        [Produces<VenueDto>]
        public IActionResult CreateVenue([FromBody] VenueDto dto)
            => throw new NotImplementedException();

        [HttpGet("{id}")]
        [Produces<VenueDto>]
        public IActionResult GetVenue([FromRoute]  string id, [FromBody] VenueDto dto)
            => throw new NotImplementedException();

        [HttpPut("{id}")]
        [Produces<VenueDto>]
        public IActionResult UpdateVenue([FromRoute] string id, [FromBody] VenueDto dto)
            => throw new NotImplementedException();

        //endpoint for a venue to get all upcoming events translated into a configuration schedule
        [HttpGet("")]
        public IActionResult GetVenueConfigurationSchedule()
            => throw new NotImplementedException();
    }
}

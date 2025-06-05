using ConcertoReservoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;

namespace ConcertoReservoApi.Controllers
{
    [Route("venues")]
    [Authorize("venues")]
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

        private readonly IVenueService _venueService;

        //powers public facing venue selection UIs
        [AllowAnonymous]
        [HttpGet("public")]
        [ProducesResponseType<PublicVenueListView[]>(200)]
        public IActionResult GetAllPublicVenues()
        {
            var venues = _venueService.GetPublicVenues();
            return Json(venues);
        }

        //
        [HttpGet("")]
        [ProducesResponseType<PublicVenueListView[]>(200)]
        public IActionResult GetAllVenues()
        {
            var venues = _venueService.GetAllVenuesForCms();
            return Json(venues);
        }

        [HttpPost("")]
        [ProducesResponseType<VenueDto>(200)]
        public IActionResult CreateVenue([FromBody] VenueDto dto)
        {
            var user = this.GetUser();
            var venue = _venueService.CreateVenue(user, dto);
            return Json(venue);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<VenueDto>(200)]
        public IActionResult GetVenue([FromRoute] string id)
        {
            var venue = _venueService.GetVenue(id);
            if (venue == null)
                return NotFound();
            return Json(venue);
        }

        [HttpPut("{id}")]
        [Produces<VenueDto>]
        public IActionResult UpdateVenue([FromRoute] string id, [FromBody] VenueDto dto)
        {
            var user = this.GetUser();
            var success = _venueService.UpdateVenue(user, id, dto);
            if (!success)
                return BadRequest();

            return Json(_venueService.GetVenue(id));
        }

        //(stretch) endpoint for a venue to get all upcoming events translated into a configuration schedule
        [HttpGet("{id}/configurationSchedule")]
        public IActionResult GetVenueConfigurationSchedule()
            => throw new NotImplementedException();
    }
}

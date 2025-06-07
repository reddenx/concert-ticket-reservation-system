using ConcertoReservoApi.Infrastructure.Dtos.Venues;
using ConcertoReservoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;

namespace ConcertoReservoApi.Controllers
{
    //I was able to spend the least amount of time on this endpoint and supporting services

    [Route("venues")]
    [Authorize("venues")]
    public class VenueController : Controller
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        //powers public facing venue selection UIs
        [AllowAnonymous]
        [HttpGet("public")]
        [ProducesResponseType<PublicVenueListView[]>(200)]
        public IActionResult GetAllPublicVenues()
        {
            var venues = _venueService.GetPublicVenues();
            return Json(venues);
        }

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

        [HttpPut("")]
        [ProducesResponseType<VenueDto>(200)]
        public IActionResult UpdateVenue([FromBody] VenueDto dto)
        {
            var user = this.GetUser();
            var venue = _venueService.UpdateVenue(user, dto);
            if (venue == null)
                return BadRequest();

            return Json(venue);
        }

        [HttpPut("{id}/sections")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult ConfigureVenue([FromRoute] string id, [FromBody] VenueSectionDto[] dtos)
        {
            var user = this.GetUser();
            var success = _venueService.UpdateVenueSections(user, id, dtos);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpGet("{id}/sections")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType<VenueSectionDto[] >(200)]
        public IActionResult GetVenueConfiguration([FromRoute] string id)
        {
            var user = this.GetUser();
            var sections = _venueService.GetVenueSections(id);
            if (sections == null)
                return NotFound();

            return Json(sections);
        }

        //(stretch) endpoint for a venue to get all upcoming events translated into a configuration schedule
        //[HttpGet("{id}/configurationSchedule")]
        //public IActionResult GetVenueConfigurationSchedule()
        //    => throw new NotImplementedException();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ConcertoReservoApi.Controllers
{
    /// <summary>
    /// does two main things
    /// - provides public events for searching and selecting
    /// - provides the crud endpoints for management of events
    /// </summary>
    [Route("events")]
    public class EventsController : Controller
    {
        public class PublicEventView
        {
            public string EventId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string VenueName { get; set; }
            public string VenueId { get; set; }

            public bool CanShopForTickets { get; set; }
            public bool CanPurchaseTickets { get; set; }

            public DateTime TicketSaleStartDate { get; set; }
            public DateTime EventDate { get; set; }
        }

        public class EventDto
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public EventPublishStates PublishState { get; set; }
            
            public DateTime TicketSaleStartDate { get; set; }
            public DateTime EventDate { get; set; }

            //configured separately to allow the soft closing of ticket sales while people are in the shopping process
            public bool? OverrideTicketsShoppable { get; set; } //allows users to start a shopping session/users to enter seating selection (pauses queue)
            public bool? OverrideTicketsPurchasable { get; set; } //allows users to purchase seats

            public VenueConfigurationDto VenueConfiguration { get; set; }

            public enum EventPublishStates { Draft, Published }
            public class VenueConfigurationDto
            {
                public string[] SelectedSectionIds { get; set; }
            }
        }

        //public and listing info

        [AllowAnonymous]
        [HttpGet("public/upcoming")]
        [ProducesResponseType<PublicEventView[]>(200)]
        public IActionResult GetUpcomingPublicEvents()
            => throw new NotImplementedException();

        [AllowAnonymous]
        [HttpGet("public/{id}")]
        [ProducesResponseType<PublicEventView>(200)]
        [ProducesResponseType(404)]
        public IActionResult GetPublicEvent([FromRoute] string id)
            => throw new NotImplementedException();

        //
        [HttpGet("{id}")]

        [HttpPost("")]
        [Produces<EventDto>]
        public IActionResult CreateEvent([FromBody] EventDto dto)
            => throw new NotImplementedException();

        [HttpPut("{id}")]
        [Produces<EventDto>]
        public IActionResult UpdateEvent([FromRoute] string id, [FromBody] EventDto dto)
            => throw new NotImplementedException();


        //
    }
}

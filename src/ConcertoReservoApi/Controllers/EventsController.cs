using ConcertoReservoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using static ConcertoReservoApi.Services.IEventsService;

namespace ConcertoReservoApi.Controllers
{
    /// <summary>
    /// does two main things
    /// - provides public events for searching and selecting
    /// - provides the crud endpoints for management of events
    /// </summary>
    [Route("events")]
    [Authorize("events")]
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
            public string Id { get; set; }
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

        private readonly IEventsService _eventsService;

        public EventsController(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        //public and listing info

        [AllowAnonymous]
        [HttpGet("public/upcoming")]
        [ProducesResponseType<PublicEventView[]>(200)]
        public IActionResult GetUpcomingPublicEvents()
        {
            var events = _eventsService.GetUpcomingPublicEvents();
            return Json(events);
        }

        [AllowAnonymous]
        [HttpGet("public/{id}")]
        [ProducesResponseType<PublicEventView>(200)]
        [ProducesResponseType(404)]
        public IActionResult GetPublicEvent([FromRoute] string id)
        {
            var publicEvent = _eventsService.GetPublicEvent(id);
            if (publicEvent == null)
                return NotFound();
            return Json(publicEvent);
        }

        //

        [HttpPost("")]
        [Produces<EventDto>]
        public IActionResult CreateEvent([FromBody] EventDto dto)
        {
            var user = this.GetUser();
            var newEvent = _eventsService.CreateEvent(user, dto);
            if (newEvent == null)
                return TranslateError(newEvent.Error.Value);
            return Json(newEvent);
        }

        [HttpPut("")]
        [Produces<EventDto>]
        public IActionResult UpdateEvent([FromBody] EventDto dto)
        {
            var user = this.GetUser();
            var updatedEvent = _eventsService.UpdateEvent(user, dto);
            if (updatedEvent.HasErrors)
                return TranslateError(updatedEvent.Error.Value);
            return Json(updatedEvent);
        }


        private IActionResult TranslateError(EventServiceErrors value)
        {
            switch (value)
            {
                case IEventsService.EventServiceErrors.NotFound:
                    return NotFound();
                case IEventsService.EventServiceErrors.BadInput:
                    return BadRequest();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

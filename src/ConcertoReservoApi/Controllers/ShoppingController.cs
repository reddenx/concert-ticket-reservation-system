using Microsoft.AspNetCore.Mvc;
using System;

namespace ConcertoReservoApi.Controllers
{
    [Route("shopping")]
    public class ShoppingController : Controller
    {
        public class ShoppingSessionDto
        {
            enum ShoppingSessionStates
            {
                Queued,
                Selecting,
                Closed
            }

            //ids
            string Id;
            string EventId;
            string? UserId;

            //state info
            ShoppingSessionStates State;
            DateTime? Expiration;

            //payment info
            string PaymentToken; //payment provider token

            //product selection

        }
        public class AvailableSeatsDto { }
        public class SeatSelectionDto { }

        [HttpPost("{eventId}")]
        [Produces<ShoppingSessionDto>]
        public IActionResult StartShoppingSession([FromRoute] string eventId)
            => throw new NotImplementedException();

        //get shopping session dto
        [HttpGet("{id}")]
        [Produces<ShoppingSessionDto>]
        public IActionResult GetShoppingSession([FromRoute] string id)
            => throw new NotImplementedException();

        //get seats for event
        [HttpGet("{id}/seating")]
        [Produces<AvailableSeatsDto>]
        public IActionResult GetSeatingAvailableForSession([FromRoute] string id)
            => throw new NotImplementedException();

        //add seats to session (with confirmation of reservation and set date)
        [HttpPut("{id}/seating")]
        [Produces<ShoppingSessionDto>]
        public IActionResult SelectSeats([FromRoute] string id, SeatSelectionDto dto)
            => throw new NotImplementedException();

        [HttpPut("{id}/payment")]
        public IActionResult VendorSpecificPaymentInput([FromRoute] string id, [FromBody] string paymentTokenizationId)
            => throw new NotImplementedException();

        //checkout action
        [HttpPost("{id}/checkout")]
        public IActionResult Checkout([FromRoute] string id)
            => throw new NotImplementedException();

    }
}

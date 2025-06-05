using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;

namespace ConcertoReservoApi.Controllers
{
    [Route("shopping")]
    public class ShoppingController : Controller
    {
        //readonly base shopping aggregate
        public class ShoppingSessionView
        {
            //ids
            public string EventId { get; set; }

            //state info
            public ShoppingSessionStates State { get; set; }
            public DateTime? Expiration {  get; set; }

            //product selection
            public SeatSelectionDto[] SelectedSeatIds { get; set; }

            //shopper info
            public ShopperDto Shopper {  get; set; }

            //payment info
            public string PaymentToken { get; set; } //payment provider token
            public PriceLineItemView[] Price {  get; set; }

            //validation
            public ValidationIssues[] CurrentValidationIssues { get; set; }

            //checkout
            public ReceiptView Receipt { get; set; }
        }

        public class PriceLineItemView
        {
            public string Label { get; set; }
            public decimal Amount { get; set; }
        }

        public class ReceiptView
        {
            public string PublicTransactionId { get; set; }
            public PriceLineItemView[] Lines { get; set; }
            public decimal AmountCaptured { get; set; }
            public DateTime PurchaseDate { get; set; }

            public string[] PrintablePurchasedSeatIds { get; set; }
        }

        public enum ShoppingSessionStates { Queued, Selecting, Expired, Purchasing, Complete }
        public class ShopperDto
        {
            public string ExistingUserId { get; set; }

            public string FullName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string[] AffilateCodes { get; set; }
        }
        public class AvailableEventSeatsView
        {
            public string Label { get; set; }
            public SectionView[] Sections { get; set; }

            public class SectionView
            {
                public string Label { get; set; }
                public SeatView[] Seats { get; set; }

                //display info
                public Point[] DisplayPolygon { get; set; }
                public Point SeatOrigin { get; set; }

                public class SeatView
                {
                    public string Id { get; set; }
                    public string Label { get; set; }
                    public SeatStates State { get; set; }
                    public int? AvailableCount { get; set; } //if applicable for seat configuration
                    public PriceLineItemView[] Price { get; set; }
                    public SeatAddonView[] Addons { get; set; }

                    //display info
                    public Point Position;

                    public enum SeatStates { Available, Reserved, Purchased } //maybe hide reserved as purchased for shopping audiences

                    public class SeatAddonView 
                    {
                        public string Id { get; set; }
                        public string Label { get; set; }
                        public PriceLineItemView[] Price { get; set; }
                    }
                }
            }
        }

        public class SeatSelectionDto
        {
            public string SeatId { get; set; }
            public string[] AddonIds { get; set; } //todo stuff like senior, disabled, promo, insurance?, etc
        }

        public enum ValidationIssues
        {
            Expired,
            MissingShopperInfo,
            InvalidShopperInfo,
            MissingSeatSelection,
            InvalidSeatSelection, //ideally "*invalid" would be broken out into exact fixable reasons to drive workflow
            MissingPaymentInfo,
        };

        [HttpPost("{eventId}")]
        [ProducesResponseType<ShoppingSessionView>(200)]
        [ProducesResponseType(429)] //duplication detection
        [ProducesResponseType(404)]
        public IActionResult StartShoppingSession([FromRoute] string eventId)
            => throw new NotImplementedException();

        //get shopping session dto
        [HttpGet("{id}")]
        [ProducesResponseType<ShoppingSessionView>(200)]
        [ProducesResponseType(404)]
        public IActionResult GetShoppingSession([FromRoute] string id)
            => throw new NotImplementedException();

        //shopper

        //update shopper info
        [HttpPut("{id}/shopper")]
        [ProducesResponseType<ShoppingSessionView>(200)]
        [ProducesResponseType(400)] //should probably quantify these issues
        [ProducesResponseType(404)]
        public IActionResult UpdateShopper([FromRoute] string id, [FromBody] ShopperDto shopper)
            => throw new NotImplementedException();


        //get seats for event, may want to live on events controller?, like an product catalog sort of thing, maybe stays in shopping as it has context of current shopper state like affiliations and discounts?
        [HttpGet("{id}/seating")]
        [ProducesResponseType<AvailableEventSeatsView>(200)]
        [ProducesResponseType(404)]
        public IActionResult GetSeatingAvailableForSession([FromRoute] string id)
            => throw new NotImplementedException();

        //add seats to session (with confirmation of reservation and set date)
        [HttpPut("{id}/seating")]
        [ProducesResponseType<ShoppingSessionView>(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)] //seat unable to be reserved, it's been taken
        public IActionResult SelectSeats([FromRoute] string id, SeatSelectionDto dto)
            => throw new NotImplementedException();



        //this depends on vendor integration, I'm assuming there's a pure frontend tokenization of PCI data providing a token reference to be used as a back-end integration when capturing payment.
        [HttpPut("{id}/payment")]
        [ProducesResponseType(204)] //ideally this is an authoriative endpoint from a trusted back-channel integration, probaly wouldn't exist on this controller but leaving here for this exercise
        [ProducesResponseType(404)]
        public IActionResult VendorSpecificPaymentInput([FromRoute] string id, [FromBody] string paymentTokenizationId)
            => throw new NotImplementedException();


        //checkout action
        [HttpPost("{id}/checkout")]
        [ProducesResponseType<ShoppingSessionView>(200)] //populated with receipt on success
        [ProducesResponseType(425)] //duplicate checkout
        [ProducesResponseType(400)] //has validations that need addressing OR a checkout error has occurred (which would be put onto validations as well)
        [ProducesResponseType(404)]
        public IActionResult Checkout([FromRoute] string id)
            => throw new NotImplementedException();



        //more of a development tool, probably not necessary with swagger working
        [HttpGet("validations")]
        [ProducesResponseType<ValidationIssues[]>(200)]
        public IActionResult GetAllValidationIssues()
            => throw new NotImplementedException();
    }
}

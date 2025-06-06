using ConcertoReservoApi.Infrastructure.Dtos;
using ConcertoReservoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using static ConcertoReservoApi.Infrastructure.Dtos.ShoppingSessionView;

namespace ConcertoReservoApi.Controllers;

[Route("shopping")]
public class ShoppingController : Controller
{
    private readonly IShoppingService _shoppingService;

    public ShoppingController(IShoppingService shoppingService)
    {
        _shoppingService = shoppingService;
    }

    [HttpPost("{eventId}")]
    [ProducesResponseType<ShoppingSessionView>(200)]
    [ProducesResponseType(429)] //duplication detection, probably more an infrastructure concern
    [ProducesResponseType(404)]
    public IActionResult StartShoppingSession([FromRoute] string eventId)
    {
        var newSession = _shoppingService.StartShopping(eventId);
        if (newSession.Error.HasValue)
            return TranslateError(newSession.Error.Value);

        return Json(newSession.Data);
    }

    //get shopping session dto
    [HttpGet("{id}")]
    [ProducesResponseType<ShoppingSessionView>(200)]
    [ProducesResponseType(404)]
    public IActionResult GetShoppingSession([FromRoute] string id)
    {
        var session = _shoppingService.GetSession(id);
        if (session.Error.HasValue)
            return TranslateError(session.Error.Value);

        return Json(session.Data);
    }

    //shopper

    //update shopper info
    [HttpPut("{id}/shopper")]
    [ProducesResponseType<ShoppingSessionView>(200)]
    [ProducesResponseType(400)] //should probably quantify these issues
    [ProducesResponseType(404)]
    public IActionResult UpdateShopper([FromRoute] string id, [FromBody] ShopperDto shopper)
    {
        var session = _shoppingService.UpdateShopper(id, shopper);
        if (session.Error.HasValue)
            return TranslateError(session.Error.Value);

        return Json(session.Data);
    }


    //get seats for event, may want to live on events controller?, like an product catalog sort of thing, maybe stays in shopping as it has context of current shopper state like affiliations and discounts?
    [HttpGet("{id}/seating")]
    [ProducesResponseType<AvailableEventSeatsView>(200)]
    [ProducesResponseType(404)]
    public IActionResult GetSeatingAvailableForSession([FromRoute] string id)
    {
        var session = _shoppingService.GetAvailableSeating(id);
        if (session.Error.HasValue)
            return TranslateError(session.Error.Value);

        return Json(session.Data);
    }

    //add seats to session (with confirmation of reservation and set date)
    [HttpPut("{id}/seating")]
    [ProducesResponseType<ShoppingSessionView>(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)] //seat unable to be reserved, it's been taken
    public IActionResult SelectSeats([FromRoute] string id, SeatSelectionDto[] dto)
    {
        var session = _shoppingService.SelectSeating(id, dto);
        if (session.Error.HasValue)
            return TranslateError(session.Error.Value);

        return Json(session.Data);
    }



    //this depends on vendor integration, I'm assuming there's a pure frontend tokenization of PCI data providing a token reference to be used as a back-end integration when capturing payment.
    [HttpPut("{id}/payment")]
    [ProducesResponseType(204)] //ideally this is an authoriative endpoint from a trusted back-channel integration, probaly wouldn't exist on this controller but leaving here for this exercise
    [ProducesResponseType(400)] //couldn't externally validate
    [ProducesResponseType(404)]
    public IActionResult VendorSpecificPaymentInput([FromRoute] string id, [FromBody] string paymentTokenizationId)
    {
        var result = _shoppingService.UpdatePaymentInformation(id, paymentTokenizationId);
        if (result.Error.HasValue)
            return TranslateError(result.Error.Value);

        return NoContent();
    }


    //checkout action
    [HttpPost("{id}/checkout")]
    [ProducesResponseType<ShoppingSessionView>(200)] //populated with receipt on success
    [ProducesResponseType(425)] //duplicate checkout
    [ProducesResponseType(400)] //has validations that need addressing OR a checkout error has occurred (which would be put onto validations as well)
    [ProducesResponseType(404)]
    public IActionResult AttemptPurchase([FromRoute] string id)
    {
        var result = _shoppingService.AttemptPurchase(id);
        if (result.Error.HasValue)
            return TranslateError(result.Error.Value);

        return NoContent();
    }

    private IActionResult TranslateError(IShoppingService.ShoppingErrors value)
    {
        switch (value)
        {
            case IShoppingService.ShoppingErrors.NotFound:
                return NotFound();
            case IShoppingService.ShoppingErrors.DuplicateSessionCreated:
                return StatusCode(429);
            default:
                throw new NotImplementedException();
        }
    }
}

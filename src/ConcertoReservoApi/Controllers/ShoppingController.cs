using ConcertoReservoApi.Infrastructure.Dtos.Shopping;
using ConcertoReservoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using static ConcertoReservoApi.Infrastructure.Dtos.Shopping.ShoppingSessionView;

namespace ConcertoReservoApi.Controllers;

[Route("shopping")]
public class ShoppingController : Controller
{
    private readonly IShoppingService _shoppingService;

    public ShoppingController(IShoppingService shoppingService)
    {
        _shoppingService = shoppingService;
    }

    /// <summary>
    /// starts a shopping session for an event
    /// </summary>
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

    /// <summary>
    /// gets shopping session information
    /// </summary>
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

    /// <summary>
    /// updates shopper details
    /// </summary>
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


    /// <summary>
    /// gets the available seating and options for a shopping session
    /// </summary>
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

    /// <summary>
    /// attaches selected available seats to a shopping session and marks them reserved.
    /// this starts the session expiration timer if it hasn't already begun.
    /// </summary>
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

    /// <summary>
    /// this depends on vendor integration, I'm assuming there's a pure frontend tokenization of PCI data providing a token reference to be used as a back-end integration when capturing payment.
    /// ideally this is an authoriative endpoint from a trusted back-channel integration, probaly wouldn't exist on this controller but leaving here for this exercise
    /// </summary>
    [HttpPut("{id}/paymentintegration")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)] //couldn't externally validate
    [ProducesResponseType(404)]
    public IActionResult VendorSpecificPaymentInput([FromRoute] string id, [FromBody] string paymentTokenizationId)
    {
        var result = _shoppingService.UpdatePaymentInformation(id, paymentTokenizationId);
        if (result.Error.HasValue)
            return TranslateError(result.Error.Value);

        return NoContent();
    }

    /// <summary>
    /// attempts to purchase selected seats on a shopping session
    /// the user confirmed price is generally a minimum for purchasing compliance and a good practice to confirm there isn't any front-end state desynchronization from the back-end
    /// </summary>
    [HttpPost("{id}/checkout")]
    [ProducesResponseType<ShoppingSessionView>(200)] //populated with receipt on success
    [ProducesResponseType(400)] //has validations that need addressing OR a checkout error has occurred (which would be put onto validations as well)
    [ProducesResponseType(404)]
    public IActionResult AttemptPurchase([FromRoute] string id, [FromQuery] decimal? userConfirmedPrice) //query probably isn't best here I should probably add a tiny dto
    {
        if (!userConfirmedPrice.HasValue)
            return BadRequest();

        var result = _shoppingService.AttemptPurchase(id, userConfirmedPrice.Value);
        if (result.Error.HasValue)
            return TranslateError(result.Error.Value);

        return NoContent();
    }

    private IActionResult TranslateError(IShoppingService.ShoppingErrors value) => value switch
    {
        IShoppingService.ShoppingErrors.CannotCheckoutWithValidationIssues => StatusCode(400, "Shopping Session Has Validation Issues"),
        IShoppingService.ShoppingErrors.DuplicateSessionCreated => StatusCode(429),
        IShoppingService.ShoppingErrors.NotFound => StatusCode(404),
        IShoppingService.ShoppingErrors.PaymentCaptureFailed => StatusCode(400, "Payment Capture Failed"),
        IShoppingService.ShoppingErrors.SelectedSeatTaken => StatusCode(209),
        IShoppingService.ShoppingErrors.TechnicalError => StatusCode(500),
        IShoppingService.ShoppingErrors.UnacceptablyBadInput => StatusCode(400),
        _ => StatusCode(500)
    };
}

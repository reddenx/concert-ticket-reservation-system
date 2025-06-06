using System;

namespace ConcertoReservoApi.Infrastructure.Dtos;

public enum ShoppingSessionStates { Queued, Selecting, Expired, Purchasing, Complete }

//readonly base shopping aggregate
public class ShoppingSessionView
{
    //ids
    public string EventId { get; set; }

    //state info
    public ShoppingSessionStates State { get; set; }
    public DateTime? Expiration { get; set; }

    //product selection
    public SeatSelectionDto[] SelectedSeatIds { get; set; }

    //shopper info
    public ShopperDto Shopper { get; set; }

    //payment info
    public string PaymentToken { get; set; } //payment provider token
    public PriceLineItemView[] Price { get; set; }

    //validation
    public ValidationIssues[] CurrentValidationIssues { get; set; }

    //checkout
    public ReceiptView Receipt { get; set; }
}
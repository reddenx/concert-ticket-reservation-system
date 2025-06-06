using System;

namespace ConcertoReservoApi.Infrastructure.Dtos;

public class ReceiptView
{
    public string PublicTransactionId { get; set; }
    public PriceLineItemView[] Lines { get; set; }
    public decimal AmountCaptured { get; set; }
    public DateTime PurchaseDate { get; set; }

    public string[] PrintablePurchasedSeatIds { get; set; }
}
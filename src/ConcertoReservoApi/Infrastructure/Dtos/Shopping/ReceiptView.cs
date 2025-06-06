using ConcertoReservoApi.Core;
using System;
using System.Linq;
using static ConcertoReservoApi.Infrastructure.ISeatingRepository;

namespace ConcertoReservoApi.Infrastructure.Dtos.Shopping;

public class ReceiptView
{
    public string PublicTransactionConfirmationCode { get; set; }

    public PriceLineItemView[] Lines { get; set; }
    public decimal AmountCaptured { get; set; }
    public DateTime PurchaseDate { get; set; }

    public string[] PrintablePurchasedSeatIds { get; set; }

    public static ReceiptView FromCore(ShoppingSession session, SeatPurchaseCode[] seatPurchases)
    {
        if (session.State != ShoppingStates.PurchaseComplete)
            return null;

        return new ReceiptView
        {
            AmountCaptured = session.PurchaceReceipt.CaptureAmount,
            PurchaseDate = session.PurchaceReceipt.PurchaseDate,
            PrintablePurchasedSeatIds = seatPurchases.Select(s => s.PurchaserReference).ToArray(),
            PublicTransactionConfirmationCode = session.PurchaceReceipt.PaymentConfirmationCode,
            Lines = session.PurchaceReceipt.LineItems.Select(l => new PriceLineItemView()
            {
                Amount = l.amount,
                Label = l.label,
            }).ToArray(),
        };
    }
}
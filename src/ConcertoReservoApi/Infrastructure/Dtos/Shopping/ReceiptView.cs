using ConcertoReservoApi.Core;
using System;
using System.Linq;
using static ConcertoReservoApi.Infrastructure.DataRepositories.ISeatingRepository;

namespace ConcertoReservoApi.Infrastructure.Dtos.Shopping;

public class ReceiptView
{
    public string PublicTransactionConfirmationCode { get; set; }

    public PriceLineItemView[] Lines { get; set; }
    public decimal AmountCaptured { get; set; }
    public DateTime PurchaseDate { get; set; }

    public string[] PrintablePurchasedSeatCodes { get; set; }

    public static ReceiptView FromCore(ShoppingSession session)
    {
        if (session.State != ShoppingStates.PurchaseComplete)
            return null;

        return new ReceiptView
        {
            AmountCaptured = session.PurchaseReceipt.CaptureAmount,
            PurchaseDate = session.PurchaseReceipt.PurchaseDate,
            PrintablePurchasedSeatCodes = session.PurchasedSeats.Select(s => s.PurchasedSeatReferenceCode).ToArray(),
            PublicTransactionConfirmationCode = session.PurchaseReceipt.PaymentConfirmationCode,
            Lines = session.PurchaseReceipt.LineItems.Select(l => new PriceLineItemView()
            {
                Amount = l.amount,
                Label = l.label,
            }).ToArray(),
        };
    }
}
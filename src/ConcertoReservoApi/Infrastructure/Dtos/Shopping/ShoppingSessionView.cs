using ConcertoReservoApi.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using static ConcertoReservoApi.Core.ShoppingSession;

namespace ConcertoReservoApi.Infrastructure.Dtos.Shopping;


//readonly base shopping aggregate
public class ShoppingSessionView
{
    //ids
    public string ShoppingSessionId { get; set; }
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
    public ValidationIssuesDto[] CurrentValidationIssues { get; set; }

    //checkout
    public ReceiptView Receipt { get; set; }

    public enum ValidationIssuesDto
    {
        Expired,
        MissingShopperInfo,
        InvalidShopperInfo,
        MissingSeatSelection,
        InvalidSeatSelection, //ideally "*invalid" would be broken out into exact fixable reasons to drive workflow
        MissingPaymentInfo,
        CurrentlyAttemptingPurchase,
        InQueue,
        AlreadyPurchased
    };
    public class ShopperDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string[] AffilateCodes { get; set; }
    }
    public class SeatSelectionDto
    {
        public string SeatId { get; set; }
        public string[] AddonIds { get; set; } //todo stuff like senior, disabled, promo, insurance?, etc
    }
    public enum ShoppingSessionStates { Queued, Selecting, Expired, Purchasing, Complete }

    public static ShoppingSessionView FromCore(ShoppingSession session, DateTimeOffset now)
    {
        return new ShoppingSessionView()
        {
            ShoppingSessionId = session.Id,
            EventId = session.EventId,
            Expiration = session.Expiration.HasValue ? session.Expiration.Value.UtcDateTime : null,
            PaymentToken = session.PaymentReference,
            Shopper = new ShopperDto
            {
                FullName = session.Shopper?.FullName,
                AffilateCodes = session.Shopper?.AffiliateCodes,
                Email = session.Shopper?.Email,
                PhoneNumber = session.Shopper?.PhoneNumber,
            },
            SelectedSeatIds = session.SelectedSeats.Select(s => new SeatSelectionDto
            {
                AddonIds = s.Addons.Select(a => a.addonId).ToArray(),
                SeatId = s.SeatId,
            }).ToArray(),
            Price = session.BuildReceipt().Select(l => new PriceLineItemView()
            {
                Amount = l.amount,
                Label = l.label
            }).ToArray(),
            State = Map(session.State),
            CurrentValidationIssues = session.GetValidationIssues(now).Select(Map).ToArray(),
            Receipt = session.PurchaseReceipt == null ? null : 
            new ReceiptView
            {
                AmountCaptured = session.PurchaseReceipt.CaptureAmount,
                PrintablePurchasedSeatCodes = session.PurchasedSeats.Select(s => s.PurchasedSeatReferenceCode).ToArray(),
            }
        };
    }
    private static ShoppingSessionStates Map(ShoppingStates shoppingStates) => shoppingStates switch
    {
        ShoppingStates.Queued => ShoppingSessionStates.Queued,
        ShoppingStates.SelectingSeats => ShoppingSessionStates.Selecting,
        ShoppingStates.Expired => ShoppingSessionStates.Expired,
        ShoppingStates.CheckingOut => ShoppingSessionStates.Purchasing,
        ShoppingStates.PurchaseComplete => ShoppingSessionStates.Complete,
        _ => throw new ArgumentOutOfRangeException($"{shoppingStates} not in {typeof(ShoppingSessionStates)}")
    };
    private static ValidationIssuesDto Map(ShoppingSessionValidations validation) => validation switch
    {
        ShoppingSessionValidations.AlreadyPurchased => ValidationIssuesDto.AlreadyPurchased,
        ShoppingSessionValidations.CurrentlyAttemptingPurchase => ValidationIssuesDto.CurrentlyAttemptingPurchase,
        ShoppingSessionValidations.InQueue => ValidationIssuesDto.InQueue,
        ShoppingSessionValidations.MissingSeatSelection => ValidationIssuesDto.MissingSeatSelection,
        ShoppingSessionValidations.MissingShopperInfo => ValidationIssuesDto.MissingShopperInfo,
        ShoppingSessionValidations.SessionExpired => ValidationIssuesDto.Expired,
        ShoppingSessionValidations.MissingPaymentInfo => ValidationIssuesDto.MissingPaymentInfo,
        _ => throw new ArgumentOutOfRangeException($"{validation} not in {typeof(ValidationIssuesDto)}")
    };
}
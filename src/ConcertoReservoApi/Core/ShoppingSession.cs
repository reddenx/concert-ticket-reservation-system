using ConcertoReservoApi.Controllers;
using System.Collections.Generic;
using System;
using ConcertoReservoApi.Infrastructure.Dtos;
using System.Linq;
using ConcertoReservoApi.Infrastructure;

namespace ConcertoReservoApi.Core;

public record SelectedSeatAddon(string addonId, string Label, decimal Price);
public record SeatSelection(string SeatId, string Label, SelectedSeatAddon[] Addons, decimal Price);
public record Shopper(string FullName, string Email, string PhoneNumber, string[] AffiliateCodes);
public record Receipt(string PaymentConfirmationCode, ReceiptLineItem[] LineItems, decimal CaptureAmount, DateTime PurchaseDate);
public record ReceiptLineItem(string label, decimal amount);
public enum ShoppingStates { Queued, SelectingSeats, CheckingOut, Expired, PurchaseComplete }
public enum ShoppingSessionValidations
{
    MissingShopperInfo,
    MissingSeatSelection,
    AlreadyPurchased,
    CurrentlyAttemptingPurchase,
    InQueue,
    SessionExpired
}

//improvements:
//- would have all mutation actions return results with handling in service layer
//- separate factory that runs through all logic from data inputs
//- in a more mature enterprise there'd be some eventing infrastructure set up to build up on updates and fire off on save, driving other domain reactions
public class ShoppingSession
{
    private const double EXPIRATION_MINUTES = 10;

    public string Id { get; }
    public string EventId { get; }
    public int Version { get; private set; }
    public ShoppingStates State { get; private set; }
    private bool _dirty = false;

    public DateTimeOffset? Expiration { get; private set; }

    private List<SeatSelection> _selectedSeats = new List<SeatSelection>();
    public SeatSelection[] SelectedSeats => _selectedSeats.ToArray();

    private List<ShoppingSessionValidations> _validationIssues = new List<ShoppingSessionValidations>();
    public ShoppingSessionValidations[] ValidationIssues => _validationIssues.ToArray();

    public Shopper Shopper { get; private set; }

    public string PaymentReference { get; private set; }
    //improvement: move to receipt object persisted separately
    public Receipt PurchaceReceipt { get; private set; }

    public ShoppingSession(string id, string eventId)
    {
        Id = id;
        EventId = eventId;
        Version = 0;
        State = ShoppingStates.SelectingSeats;
        _dirty = false;
        
    }

    public void ClearSelectedSeats()
    {
        if (_selectedSeats.Any())
        {
            _selectedSeats.Clear();
            _dirty = true;
        }
    }

    public void AddSelectedSeating(SeatSelection selection)
    {
        _selectedSeats.Add(selection);
        _dirty = true;
    }

    public void PaymentCaptureSucceeded(string confirmationCode, decimal amountCaptured, ReceiptLineItem[] receipt, DateTime purchaseDate)
    {
        this.PurchaceReceipt = new Receipt(confirmationCode, receipt, amountCaptured, purchaseDate);
        State = ShoppingStates.PurchaseComplete;
        _dirty = true;
    }

    public void PaymentCaptureFailed()
    {
        State = ShoppingStates.SelectingSeats;
    }

    public void AttachPaymentToken(string paymentToken)
    {
        PaymentReference = paymentToken;
        _dirty = true;
    }

    public void EnsureCountdownStarted(DateTimeOffset now)
    {
        if (!Expiration.HasValue)
        {
            Expiration = now.AddMinutes(EXPIRATION_MINUTES);
            _dirty = true;
        }
    }

    public void PurchaseStarted()
    {
        if (State != ShoppingStates.CheckingOut)
        {
            State = ShoppingStates.CheckingOut;
            _dirty = true;
        }
    }

    //improvement: move to pricing/discount system that would apply affiliation things
    public ReceiptLineItem[] BuildReceipt()
    {
        //improvement: more concrete relationship between offerings/selections and receipt line items
        var lineItems = new List<ReceiptLineItem>();
        foreach (var seat in _selectedSeats)
        {
            lineItems.Add(new ReceiptLineItem(seat.Label, seat.Price));
            foreach (var addon in seat.Addons)
            {
                lineItems.Add(new ReceiptLineItem(addon.Label, addon.Price));
            }
        }
        return lineItems.ToArray();
    }

    internal void UpdateShopper(string email, string fullName, string phoneNumber, string[] affilateCodes)
    {
        this.Shopper = new Shopper(fullName, email, phoneNumber, affilateCodes);
        _dirty = true;
    }


    public ShoppingSessionValidations[] GetValidationIssues(DateTime now)
    {
        //improvements:
        //- rules engine pattern (command or class pattern with descriptively named checkers like "CannotMakePurchaseIfInQueue")

        var validationIssues = new List<ShoppingSessionValidations>();

        //value validations
        //- info missing
        //- info invalid (skipping for time)

        if (string.IsNullOrWhiteSpace(Shopper?.Email)
            || string.IsNullOrWhiteSpace(Shopper?.FullName)
            || string.IsNullOrWhiteSpace(Shopper?.PhoneNumber))
            validationIssues.Add(ShoppingSessionValidations.MissingShopperInfo);
        if (!SelectedSeats.Any())
            validationIssues.Add(ShoppingSessionValidations.MissingSeatSelection);

        //state validation
        //- cannot checkout case already checking out
        //- expired

        if (State == ShoppingStates.PurchaseComplete)
            validationIssues.Add(ShoppingSessionValidations.AlreadyPurchased);
        if (State == ShoppingStates.Queued)
            validationIssues.Add(ShoppingSessionValidations.InQueue);
        if (State == ShoppingStates.CheckingOut)
            validationIssues.Add(ShoppingSessionValidations.CurrentlyAttemptingPurchase);

        if (now > Expiration)
            validationIssues.Add(ShoppingSessionValidations.SessionExpired);

        return validationIssues.ToArray();
    }

    //should be handled at data layer, but for simplicity of demo and mocking purposes it's here publicly
    public void RecordSaved()
    {
        Version += 1;
        _dirty = false;
    }
}

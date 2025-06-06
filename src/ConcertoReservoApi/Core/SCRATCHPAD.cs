using ConcertoReservoApi.Controllers;
using System;
using System.Collections.Generic;

namespace ConcertoReservoApi.Core;
/*
 * fast prototyping some domain entities
 */
//public class Id<T>
//{
//    private readonly string _id;

//    public static Id<T> Of(string id)
//    {
//        if (id == null) return null;

//        return new Id<T>(id);
//    }

//    private Id(string id)
//    {
//        _id = id;
//    }
//}

//definition of a venue
public class Venue
{
    public string Id { get; }
    public string Name { get; }

    public class Section
    {
        public string Id { get; }
        public string Label { get; }

        public class Seat
        {
            public string Id { get; }
            public string Label { get; }
        }
    }
}

//instance of a venue for an event
public class EventVenue
{
    public string VenueId { get; }
}

//the event itself
// public class Event
// {
//     EventVenue Configuration;
//     DateTime EventDate;
//     string Title;
//     string Description;
//     DateTime? PublishDate;
//     DateTime? SalesOpeningDate;
// }

public class EventSeat
{
    public string Id;
    public string Label;
    public string HeldBySessionId;

    public SeatReservationStatuses Status { get; internal set; }
    public enum SeatReservationStatuses { Available, Reserved, Purchased }
}

//user account
public class User
{
    string Firstname;
    string Lastname;
}


//the timed user experience throught the shopping queue
public class ShoppingSession
{
    private const int EXPIRATION_MINUTES = 10;

    public string Id { get; }

    public string EventId { get; }

    public record SeatSelection(string seatId, string[] addonIds, decimal price);
    private List<SeatSelection> _selectedSeats = new List<SeatSelection>();
    public SeatSelection[] SelectedSeats => _selectedSeats.ToArray();

    private List<Validations> _validationIssues = new List<Validations>();
    public Validations[] ValidationIssues => _validationIssues.ToArray();
    public enum Validations { }

    public DateTime? Expiration { get; private set; }

    public string PaymentToken { get; private set; }
    public string PaymentConfirmation { get; private set; }

    public void ClearSelectedSeats()
    {
        _selectedSeats.Clear();
    }

    public void AddSelectedSeating()
    {
        _selectedSeats.Add(new SeatSelection(seatId, addons, seatPrice));
    }

    public void AttachPaymentConfirmation(string confirmationCode)
    {
        PaymentConfirmation = confirmationCode;
    }

    public void AttachPaymentToken(string paymentToken)
    {
        PaymentToken = paymentToken;
    }

    internal void EnsureCountdownStarted(DateTime now)
    {
        if (!Expiration.HasValue)
        {
            Expiration = now.AddMinutes(EXPIRATION_MINUTES);
        }
    }

    internal decimal GetTotalPrice()
    {
        throw new NotImplementedException();
    }

    internal void StartPurchase()
    {
        throw new NotImplementedException();
    }

    internal ShoppingController.ShoppingSessionView ToDto()
    {
        throw new NotImplementedException();
    }

    internal void UpdateShopper(string email, string fullName, string phoneNumber, string[] affilateCodes)
    {
        throw new NotImplementedException();
    }

    internal void Validate()
    {
        //has 
        throw new NotImplementedException();
    }
}

//artifact of a purchase
public class Purchase { }

using ConcertoReservoApi.Controllers;
using System;
using System.Collections.Generic;

namespace ConcertoReservoApi.Core
{
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
        Id<Venue> Id;
        string Name;

        public class Section
        {
            Id<Section> Id;
            string Label;

            public class Seat
            {
                Id<Seat> Id;
                string Label;
            }
        }
    }

    //instance of a venue for an event
    public class EventVenue
    {
        Id<Venue> VenueId;
    }

    //the event itself
    public class Event
    {
        EventVenue Configuration;
        DateTime EventDate;
        string Title;
        string Description;
        DateTime? PublishDate;
        DateTime? SalesOpeningDate;
    }

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
        public string Id { get; }
        public string EventId { get; }
        public EventSeat[] SelectedSeats { get; }
        public ValidationIssue ValidationIssues { get; set; }
        public string PaymentToken { get; internal set; }

        internal void AddSelectedSeating(EventSeat eventSeat, string[] addonIds)
        {
            throw new NotImplementedException();
        }

        internal void AttachPaymentConfirmation(object confirmationCode)
        {
            throw new NotImplementedException();
        }

        internal void AttachPaymentToken(string paymentTokenizationId)
        {
            throw new NotImplementedException();
        }

        internal void ClearSelectedSeats()
        {
            throw new NotImplementedException();
        }

        internal void EnsureCountdownStarted()
        {
            throw new NotImplementedException();
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
}

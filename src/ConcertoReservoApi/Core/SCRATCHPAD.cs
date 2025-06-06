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
//public class Venue
//{
//    public string Id { get; }
//    public string Name { get; }

//    public class Section
//    {
//        public string Id { get; }
//        public string Label { get; }

//        public class Seat
//        {
//            public string Id { get; }
//            public string Label { get; }
//        }
//    }
//}

//instance of a venue for an event
//public class EventVenue
//{
//    public string VenueId { get; }
//}

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

//public class EventSeat
//{
//    public string Id;
//    public string Label;
//    public string HeldBySessionId;

//    public SeatReservationStatuses Status { get; internal set; }
//    public enum SeatReservationStatuses { Available, Reserved, Purchased }
//}

//user account
//public class User
//{
//    string Firstname;
//    string Lastname;
//}


//the timed user experience throught the shopping queue

//artifact of a purchase
//public class Purchase { }

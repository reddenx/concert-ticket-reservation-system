using System;

namespace ConcertoReservoApi.Core
{
    /*
     * fast prototyping some domain entities
     */

    public class Id<T> 
    {
        Guid _id;
    }

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

    //user account
    public class User 
    {
        string Firstname;
        string Lastname;
    }

    //the timed user experience throught the shopping queue
    public class ShoppingSession { }

    //artifact of a purchase
    public class Purchase { }
}

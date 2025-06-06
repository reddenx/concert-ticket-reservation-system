using System.Drawing;
using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Infrastructure
{
    public class PricingLineItem
    {
        public string Label { get; set; }
        public decimal Amount { get; set; }
    }
    public class Section
    {
        public string Label { get; set; }
        public Seat[] Seats { get; set; }

        //display info
        public Point[] DisplayPolygon { get; set; }
        public Point SeatOrigin { get; set; }

        public class Seat
        {
            public string Id { get; set; }
            public string Label { get; set; }

            public SeatStates State { get; set; }
            public int? AvailableCount { get; set; } //if applicable for seat configuration

            public PricingLineItem[] Price { get; set; }
            public SeatAddonView[] AddonOptions { get; set; }

            //display info
            public Point Position;

            public enum SeatStates { Available, Reserved, Purchased } //maybe hide reserved as purchased for shopping audiences

            public class SeatAddonView
            {
                public string Id { get; set; }
                public string Label { get; set; }
                public PricingLineItem[] Price { get; set; }
            }
        }
    }

    public interface ISeatingRepository
    {
        record SeatPurchaseCode(string SeatId, string PurchaserReference, string ShoppingSessionId);

        EventSeat GetSeat(string eventId, string seatId);
        AvailableSeating GetEventSeating(string eventId);
        bool ReserveSeat(string eventId, string shoppingSessionId, EventSeat seat);
        void RemoveReservation(string eventId, string shoppingSessionId, EventSeat seat);
        SeatPurchaseCode MarkSeatPurchased(string eventId, string seatId, string shoppingSessionId);
        SeatPurchaseCode GetSeatPurchaseCode(string eventId, string seatId, string shoppingSessionId);
    }
}

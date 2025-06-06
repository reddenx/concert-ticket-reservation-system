using System.Drawing;
using ConcertoReservoApi.Core;
using static ConcertoReservoApi.Infrastructure.ISeatingRepository;

namespace ConcertoReservoApi.Infrastructure
{
    public interface ISeatingRepository
    {
        record SeatPurchaseCode(string SeatId, string PurchaserReference, string ShoppingSessionId);
        record EventSection(string SectionId, string Label, EventSeat[] Seats, Point[] DisplayPolygon, Point Position);
        record EventSeat(string SeatId, SeatStatuses Status, string Label, EventSeatAddon[] AvailableAddons, decimal Price, Point Position);
        enum SeatStatuses { Available, Reserved, Purchased }
        record EventSeatAddon(string AddonId, string Label, decimal Price);

        EventSeat GetEventSeat(string eventId, string seatId);
        EventSection[] GetAvailableEventSeating(string eventId);
        bool ReserveSeat(string eventId, string shoppingSessionId, string seatId);
        void RemoveReservation(string eventId, string shoppingSessionId, string seatId);
        SeatPurchaseCode MarkSeatPurchased(string eventId, string seatId, string shoppingSessionId);
        bool HasEventHadAnySalesActivity(string eventId);
        
        void DeleteEventSeating(string eventId);
        void CreateSeatingForEvent(string eventId, string venueId, EventSectionData[] sectionConfiguration);
        SeatPurchaseCode[] GetReferenceCodesForPurchase(string id);
    }
}

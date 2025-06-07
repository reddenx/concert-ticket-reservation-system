using System.Drawing;
using ConcertoReservoApi.Core;
using static ConcertoReservoApi.Infrastructure.DataRepositories.ISeatingRepository;

namespace ConcertoReservoApi.Infrastructure.DataRepositories
{
    public interface ISeatingRepository
    {
        record SeatPurchaseCodeData(string SeatId, string PurchaserReference, string ShoppingSessionId);
        record EventSectionData(string SectionId, string Label, EventSeatData[] Seats, Point[] DisplayPolygon, Point DisplayPosition);
        record EventSeatData(string SeatId, SeatStatuses Status, string Label, EventSeatAddonData[] AvailableAddons, decimal Price, Point DisplayPosition);
        enum SeatStatuses { Available, Reserved, Purchased }
        record EventSeatAddonData(string AddonId, string Label, decimal Price);


        EventSeatData GetEventSeat(string eventId, string seatId);
        EventSectionData[] GetAvailableEventSeating(string eventId);

        bool ReserveSeat(string eventId, string shoppingSessionId, string seatId);
        void RemoveReservation(string eventId, string shoppingSessionId, string seatId);
        SeatPurchaseCodeData MarkSeatPurchased(string eventId, string seatId, string shoppingSessionId);
        bool HasEventHadAnySalesActivity(string eventId);

        void DeleteEventSeating(string eventId);
        void CreateSeatingForEvent(string eventId, string venueId, EventSectionConfigurationData[] sectionConfiguration);
        SeatPurchaseCodeData[] GetReferenceCodesForPurchase(string shoppingSessionId);
    }
}

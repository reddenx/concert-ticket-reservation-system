using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Infrastructure
{
    public interface ISeatingRepository
    {
        EventSeat GetSeat(string eventId, string seatId);
        bool ReserveSeat(string eventId, string shoppingSessionId, EventSeat seat);
        void RemoveReservation(string eventId, string shoppingSessionId, EventSeat seat);
        void MarkSeatPurchased(string eventId, string seatId, string shoppingSessionId);
    }
}

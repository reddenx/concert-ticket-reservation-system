using ConcertoReservoApi.Core;
using System;
using System.Collections.Generic;
using static ConcertoReservoApi.Controllers.ShoppingController;

namespace ConcertoReservoApi.Services
{
    public interface IShoppingService
    {
        enum ShoppingErrors { NotFound, DuplicateSessionCreated }
        record Result<T> (T Data, ShoppingErrors? Error);
        record Result(ShoppingErrors? Error);

        Result<ShoppingSessionView> StartShopping(string eventId);
        Result<ShoppingSessionView> GetSession(string sessionId);
        Result<ShoppingSessionView> UpdateShopper(string sessionId, ShopperDto shopper);
        Result<AvailableEventSeatsView> GetAvailableSeating(string sessionId);
        Result<ShoppingSessionView> SelectSeating(string sessionId, SeatSelectionDto dto);
        Result UpdatePaymentInformation(string id, string paymentTokenizationId);
        Result AttemptPurchase(string id);
    }

    public class ShoppingService : IShoppingService { }
}

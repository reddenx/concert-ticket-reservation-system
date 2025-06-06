using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Infrastructure
{
    public interface IShoppingRepository
    {
        ShoppingSession CreateShoppingSession(string eventId);
        ShoppingSession Get(string id);
        bool Save(ShoppingSession session);
    }
    public class MockShoppingRepository : IShoppingRepository { }
}

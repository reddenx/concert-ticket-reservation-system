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
    public class MockShoppingRepository : IShoppingRepository
    {
        public ShoppingSession CreateShoppingSession(string eventId)
        {
            throw new System.NotImplementedException();
        }

        public ShoppingSession Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool Save(ShoppingSession session)
        {
            throw new System.NotImplementedException();
        }
    }
}

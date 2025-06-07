using ConcertoReservoApi.Controllers;
using ConcertoReservoApi.Core;

namespace ConcertoReservoApi.Infrastructure.DataRepositories
{
    public interface IShoppingRepository
    {
        ShoppingSession CreateShoppingSession(string eventId);
        ShoppingSession Get(string id);
        bool Save(ShoppingSession session);
    }
}

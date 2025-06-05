using ConcertoReservoApi.Core;
using System;
using System.Collections.Generic;

namespace ConcertoReservoApi.Services
{
    


    public interface IShoppingService
    {
        record Result<T>;

        //Result<ShoppingSessionDto> StartSession(params object[] inputs);
        //Result<ShoppingSessionDto> Update(ShoppingSessionDto session);
        //Result<ShoppingSessionDto> Get(Id<ShoppingSession> id);
    }
}

using System;

namespace ConcertoReservoApi.Infrastructure.Dtos;

public class PriceLineItemView
{
    public string Label { get; set; }
    public decimal Amount { get; set; }
}
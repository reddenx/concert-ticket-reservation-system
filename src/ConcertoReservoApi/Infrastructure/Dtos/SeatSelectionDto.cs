using System;

namespace ConcertoReservoApi.Infrastructure.Dtos;

public class SeatSelectionDto
{
    public string SeatId { get; set; }
    public string[] AddonIds { get; set; } //todo stuff like senior, disabled, promo, insurance?, etc
}
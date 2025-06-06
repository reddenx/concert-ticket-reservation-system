using System;

namespace ConcertoReservoApi.Infrastructure.Dtos;

public class ShopperDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string[] AffilateCodes { get; set; }
}
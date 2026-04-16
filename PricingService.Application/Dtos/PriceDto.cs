namespace PricingService.Application.Dtos;

public sealed record PriceDto(
    Guid ProductId,
    long BasePriceMinor,
    string Currency,
    DiscountDto? Discount);
namespace PricingService.Application.Dtos;

public sealed record PriceDto(
    Guid ProductId,
    long BasePriceMinor,
    long CurrentPriceMinor,
    string Currency,
    DiscountDto? Discount);
namespace PricingService.Application.Dtos;

public sealed record VariantPriceDto(
    string Currency,
    bool IsBase,
    long CurrentPriceMinor);

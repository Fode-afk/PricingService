namespace PricingService.Application.Dtos;

public sealed record VariantPriceDto(
    string Market,
    long CurrentPriceMinor,
    string Currency);
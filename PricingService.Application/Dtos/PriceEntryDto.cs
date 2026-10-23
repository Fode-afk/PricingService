namespace PricingService.Application.Dtos;

public sealed record PriceEntryDto(
     long PriceMinor,
     string Currency,
     DateTimeOffset EffectiveFrom,
     DateTimeOffset? EffectiveTo);
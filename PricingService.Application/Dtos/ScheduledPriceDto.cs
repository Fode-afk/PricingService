namespace PricingService.Application.Dtos;

public sealed record ScheduledPriceDto(
     long AmountMinor,
     DateTimeOffset EffectiveFrom,
     DateTimeOffset? EffectiveTo);
namespace PricingService.Application.Dtos;

public sealed record PriceStateDto(PriceEntryDto Current, PriceEntryDto? NextScheduled);
using migApp.Shared.Enums.Discounts;

namespace PricingService.Application.Dtos;

public sealed record DiscountDto(
    Guid DiscountId,
    DiscountType Type,
    decimal? Percentage,
    long? FixedPriceMinor,
    long? AmountOffMinor,
    string? CampaignName,
    int Priority,
    bool IsStackable,
    DateTimeOffset Start,
    DateTimeOffset End);
using migApp.Shared.Enums.Discounts;

namespace PricingService.Application.Dtos;

public sealed record DiscountDto(
    Guid DiscountId,
    DiscountType Type,
    decimal? Percentage,
    long? FixedPriceMinor,
    long? AmountOffMinor,
    DateTimeOffset Start,
    DateTimeOffset End);
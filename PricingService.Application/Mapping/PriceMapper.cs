using PricingService.Application.Dtos;
using PricingService.Domain.Models;

namespace PricingService.Application.Mapping;

internal static class PriceMapper
{
    public static IReadOnlyList<PriceHistoryEntryDto> ToDtos(this List<PriceHistoryEntry> entries) =>
        [.. entries.Select(e => e.ToDto())];

    private static PriceHistoryEntryDto ToDto(this PriceHistoryEntry entry) =>
        new(
            entry.Amount.ToMinor(),
            entry.Reason.ToString(),
            entry.ReasonNote,
            entry.ChangedBy,
            entry.Source,
            entry.CreatedAt);

    public static PriceEntryDto ToDto(this PriceEntry entry) =>
        new(
            entry.Amount.ToMinor(),
            entry.Amount.Currency,
            entry.EffectiveFrom,
            entry.EffectiveTo);

    public static VariantPriceDto ToDto(this Price price, DateTimeOffset now) =>
        new(
            price.Currency,
            price.IsBase,
            price.CurrentEntry(now).Amount.ToMinor());
}
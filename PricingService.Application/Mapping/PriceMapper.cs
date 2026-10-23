using migApp.Shared.Domain.Extensions;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Application.Dtos;
using PricingService.Domain.Models;

namespace PricingService.Application.Mapping;

internal static class PriceMapper
{
    public static IReadOnlyList<PriceHistoryEntryDto> ToDtos(
        this List<PriceHistoryEntry> entries,
        decimal exchangeRate,
        Currency targetCurrency) =>
        [.. entries.Select(e => e.ToDto(exchangeRate, targetCurrency))];

    private static PriceHistoryEntryDto ToDto(
        this PriceHistoryEntry entry, 
        decimal exchangeRate,
        Currency targetCurrency) =>
        new(
            entry.Amount.ConvertToMinor(targetCurrency, exchangeRate),
            entry.Reason.ToString(),
            entry.ReasonNote,
            entry.ChangedBy,
            entry.Source,
            entry.CreatedAt);

    public static PriceEntryDto ToDto(
        this PriceEntry entry,
        decimal exchangeRate,
        Currency targetCurrency) =>
        new(
            entry.Amount.ConvertToMinor(targetCurrency, exchangeRate),
            targetCurrency,
            entry.EffectiveFrom,
            entry.EffectiveTo);
}
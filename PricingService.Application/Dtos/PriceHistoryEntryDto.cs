namespace PricingService.Application.Dtos;

public sealed record PriceHistoryEntryDto(
    long AmountMinor,
    string Reason,
    string? ReasonNote,
    Guid ChangedBy,
    string Source,
    DateTimeOffset CreatedAt);
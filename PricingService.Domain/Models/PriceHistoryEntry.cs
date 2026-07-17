using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Enums;
using PricingService.Domain.Primitives;

namespace PricingService.Domain.Models;

public sealed class PriceHistoryEntry : Entity
{
    private PriceHistoryEntry() : base(Guid.Empty) { }

    private PriceHistoryEntry(
        Guid id,
        Guid priceId,
        Money amount,
        PriceChangeReason reason,
        string? reasonNote,
        Guid changedBy,
        string source,
        DateTimeOffset createdAt) : base(id)
    {
        PriceId = priceId;
        Amount = amount;
        Reason = reason;
        ReasonNote = reasonNote;
        ChangedBy = changedBy;
        Source = source;
        CreatedAt = createdAt;
    }

    public Guid PriceId { get; private set; }

    public Money Amount { get; }

    public PriceChangeReason Reason { get; }
    public string? ReasonNote { get; }

    public Guid ChangedBy { get; }

    public string Source { get; }

    public DateTimeOffset CreatedAt { get; }

    internal static PriceHistoryEntry Record(
        Guid priceId,
        Money amount,
        PriceChangeReason reason,
        Guid changedBy,
        string source,
        DateTimeOffset now,
        string? reasonNote = null) =>
        new(
            Guid.NewGuid(),
            priceId,
            amount,
            reason,
            reasonNote,
            changedBy,
            source,
            now);
}
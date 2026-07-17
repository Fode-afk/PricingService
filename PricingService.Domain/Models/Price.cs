using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Domain.Context;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Enums;
using PricingService.Domain.Exceptions;
using PricingService.Domain.Primitives;
using PricingService.Domain.Specifications.Price;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Models;

public sealed class Price : AggregateRoot
{
    private Price() : base(Guid.Empty) { }

    private Price(
        Guid id,
        Guid productVariantId,
        Guid vendorId,
        DateTimeOffset createdAt) : base(id)
    {
        ProductVariantId = productVariantId;
        VendorId = vendorId;
        CreatedAt = createdAt;
    }

    public Guid ProductVariantId { get; private set; }
    public Guid VendorId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<PriceEntry> _entries = [];
    public IReadOnlyList<PriceEntry> Entries => _entries.AsReadOnly();

    private readonly List<PriceHistoryEntry> _history = [];
    public IReadOnlyList<PriceHistoryEntry> History => _history.AsReadOnly();

    public PriceEntry CurrentEntry(DateTimeOffset now) =>
        _entries
            .Where(e => e.IsActiveAt(now))
            .MaxBy(e => e.EffectiveFrom) ?? throw new ActivePriceNotFoundException(Id);

    public PriceEntry? NextScheduledEntry(DateTimeOffset now) =>
        _entries
            .Where(e => e.IsScheduled(now))
            .MinBy(e => e.EffectiveFrom);

    public bool HasActivePrice(DateTimeOffset now) => CurrentEntry(now) != null;

    public static IResult<Price> Create(
        PriceCreationContext ctx,
        Guid productVariantId,
        Guid vendorId,
        Money initialAmount,
        Guid createdBy,
        DateTimeOffset now,
        DateTimeOffset? effectiveFrom = null)
    {
        var result = PriceCreationSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<Price>(result.Error);

        var price = new Price(Guid.NewGuid(), productVariantId, vendorId, now);

        var entryResult = PriceEntry.Create(price.Id, initialAmount, effectiveFrom ?? now);
        if (entryResult.IsFailure)
            return Fail<Price>(entryResult.Error);

        var entry = entryResult.Value;
        price._entries.Add(entry);

        var snapshot = PriceHistoryEntry.Record(
            price.Id,
            initialAmount,
            PriceChangeReason.Initial,
            createdBy,
            source: "manual",
            now);
        price._history.Add(snapshot);

        price.RaiseDomainEvent(new PriceCreatedDomainEvent(
            price.ProductVariantId,
            price.HasActivePrice(now),
            price.Version));

        return Ok(price);
    }

    public IResult UpdatePrice(
        UpdatePriceContext ctx,
        Money newAmount,
        Guid changedBy,
        PriceChangeReason reason,
        string source,
        DateTimeOffset now,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? effectiveTo = null,
        string? reasonNote = null)
    {
        var result = UpdatePriceSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var from = effectiveFrom ?? now;

        var conflictingScheduled = _entries
            .Where(e => e.IsScheduled(now))
            .Where(e => effectiveTo == null
                ? e.EffectiveFrom >= from
                : e.EffectiveFrom >= from && e.EffectiveFrom < effectiveTo)
            .ToList();

        foreach (var conflict in conflictingScheduled)
            _entries.Remove(conflict);

        var activeEntry = _entries.FirstOrDefault(e => e.IsActiveAt(from));
        if (activeEntry is not null)
        {
            var closeResult = activeEntry.Close(from);
            if (closeResult.IsFailure)
                return closeResult;
        }

        if (effectiveTo.HasValue)
        {
            var entryAtEnd = _entries.FirstOrDefault(e => e.IsActiveAt(effectiveTo.Value));
            if (entryAtEnd is not null && entryAtEnd != activeEntry)
            {
                var closeResult = entryAtEnd.Close(effectiveTo.Value);
                if (closeResult.IsFailure)
                    return closeResult;
            }
        }

        var newEntryResult = PriceEntry.Create(Id, newAmount, from, effectiveTo);
        if (newEntryResult.IsFailure)
            return newEntryResult;

        _entries.Add(newEntryResult.Value);

        var historyEntry = PriceHistoryEntry.Record(
            Id,
            newAmount,
            reason,
            changedBy,
            source,
            now,
            reasonNote);
        _history.Add(historyEntry);

        UpdatedAt = now;
        IncreaseVersion();

        RaiseDomainEvent(new PriceUpdatedDomainEvent(
            ProductVariantId,
            HasActivePrice(now),
            Version));

        return Ok();
    }

    public IResult Archive(DateTimeOffset now)
    {
        var scheduled = _entries.Where(e => e.IsScheduled(now)).ToList();
        foreach (var entry in scheduled)
            _entries.Remove(entry);

        var activeEntry = _entries.FirstOrDefault(e => e.IsActiveAt(now));
        if (activeEntry is not null)
        {
            var result = activeEntry.Close(now);
            if (result.IsFailure)
                return result;
        }

        UpdatedAt = now;
        IncreaseVersion();

        RaiseDomainEvent(new PriceArchivedDomainEvent(ProductVariantId));

        return Ok();
    }
}
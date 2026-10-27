using migApp.Shared.Domain.Primitives;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Domain.Context;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Enums;
using PricingService.Domain.Errors;
using PricingService.Domain.Exceptions;
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
        bool isBase, 
        Currency currency,
        DateTimeOffset createdAt) : base(id)
    {
        ProductVariantId = productVariantId;
        VendorId = vendorId;
        IsBase = isBase;
        Currency = currency;
        CreatedAt = createdAt;
    }

    public Guid ProductVariantId { get; }
    public Guid VendorId { get; }

    public bool IsBase { get; }

    public Currency Currency { get; }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<PriceEntry> _entries = [];
    public IReadOnlyList<PriceEntry> Entries => _entries;

    private readonly List<PriceHistoryEntry> _history = [];
    public IReadOnlyList<PriceHistoryEntry> History => _history;

    public PriceEntry CurrentEntry(DateTimeOffset now) =>
        _entries
            .Where(e => e.IsActiveAt(now))
            .MaxBy(e => e.EffectiveFrom)
                ?? throw new ActivePriceNotFoundException(Id);

    public PriceEntry? NextScheduledEntry(DateTimeOffset now) =>
        _entries
            .Where(e => e.IsScheduled(now))
            .MinBy(e => e.EffectiveFrom);

    public static IResult<Price> Create(
        PriceCreationContext ctx,
        Guid productVariantId,
        Guid vendorId,
        Money initialAmount,
        Currency vendorBaseCurrency,
        Guid createdBy,
        DateTimeOffset now)
    {
        var result = PriceCreationSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<Price>(result.Error);

        var price = new Price(
            Guid.NewGuid(),
            productVariantId,
            vendorId,
            isBase: vendorBaseCurrency == initialAmount.Currency,
            initialAmount.Currency,
            now);

        var entryResult = PriceEntry.Create(price.Id, initialAmount, now);
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
            price.Id,
            price.ProductVariantId,
            price.IsBase,
            entry.Amount,
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
        DateTimeOffset? effectiveFrom,
        DateTimeOffset? effectiveTo,
        string? reasonNote = null)
    {
        var result = UpdatePriceSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        if (newAmount.Currency != Currency)
            return Fail(PriceErrors.CurrencyMismatch());

        var from = effectiveFrom ?? now;

        if (from < now || (effectiveTo.HasValue && effectiveTo <= from))
            return Fail(PriceEntryErrors.InvalidTerms());

        if (CurrentEntry(now) is null && (from > now || effectiveTo.HasValue))
            return Fail(PriceErrors.NoActivePrice());

        var newEntryResult = PriceEntry.Create(Id, newAmount, from, effectiveTo);
        if (newEntryResult.IsFailure)
            return newEntryResult;

        PriceEntry? tail = null;
        if (effectiveTo is { } to)
        {
            var entryAtEnd = _entries.FirstOrDefault(e => e.IsActiveAt(to));
            if (entryAtEnd is not null && entryAtEnd.EffectiveFrom < to)
            {
                var tailResult = PriceEntry.Create(Id, entryAtEnd.Amount, to, entryAtEnd.EffectiveTo);
                if (tailResult.IsFailure)
                    return tailResult;

                tail = tailResult.Value;
            }
        }

        var spanning = _entries.FirstOrDefault(e => e.EffectiveFrom < from && e.IsActiveAt(from));
        if (spanning is not null)
        {
            var closeResult = spanning.Close(from);
            if (closeResult.IsFailure)
                return closeResult;
        }

        _entries.RemoveAll(e =>
            e.EffectiveFrom >= from &&
            (effectiveTo is null || e.EffectiveFrom < effectiveTo));

        _entries.Add(newEntryResult.Value);
        if (tail is not null)
            _entries.Add(tail);

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
            Id,
            CurrentEntry(now).Amount,
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

        RaiseDomainEvent(new PriceArchivedDomainEvent(Id));

        return Ok();
    }
}
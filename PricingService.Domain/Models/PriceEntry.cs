using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Domain.Errors;
using PricingService.Domain.Primitives;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Models;

public sealed class PriceEntry : Entity
{
    private PriceEntry() : base(Guid.Empty) { }

    private PriceEntry(
        Guid id,
        Guid priceId,
        Money amount,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo) : base(id)
    {
        PriceId = priceId;
        Amount = amount;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public Guid PriceId { get; private set; }

    public Money Amount { get; private set; }

    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }
    public bool IsScheduled(DateTimeOffset now) => EffectiveFrom > now;

    public bool IsActiveAt(DateTimeOffset at) =>
        EffectiveFrom <= at && (EffectiveTo == null || EffectiveTo > at);

    internal static IResult<PriceEntry> Create(
        Guid priceId,
        Money amount,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo = null)
    {
        if (effectiveTo.HasValue && effectiveTo <= effectiveFrom)
            return Fail<PriceEntry>(PriceEntryErrors.InvalidTerms());

        return Ok(new PriceEntry(Guid.NewGuid(), priceId, amount, effectiveFrom, effectiveTo));
    }

    internal IResult Close(DateTimeOffset effectiveTo)
    {
        if (effectiveTo <= EffectiveFrom)
            return Fail(PriceEntryErrors.InvalidTerms());

        EffectiveTo = effectiveTo;

        return Ok();
    }
}

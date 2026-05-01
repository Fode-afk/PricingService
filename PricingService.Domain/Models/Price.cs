using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Errors;
using PricingService.Domain.Primitives;
using PricingService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Models;

public sealed class Price : AggregateRoot
{
    private Price() : base(Guid.Empty) { }

    private Price(
        Guid id,
        Guid productId,
        Money basePrice,
        DateTimeOffset createdAt) : base(id)
    {
        ProductId = productId;
        BasePrice = basePrice;
        CurrentPrice = basePrice;
        CreatedAt = createdAt;
    }

    public Guid ProductId { get; private set; }

    public Money BasePrice { get; private set; }
    public Money CurrentPrice { get; private set; }
    public DiscountSnapshot? AppliedDiscount { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public bool HasActiveDiscount(DateTimeOffset now)
        => AppliedDiscount is not null && AppliedDiscount.IsActive(now);

    public static IResult<Price> Create(
        Guid productId,
        Guid vendorId,
        Money basePrice,
        DateTimeOffset now)
    {
        if (basePrice is null)
            return Fail<Price>(PricingErrors.BasePriceIsRequired());

        var price = new Price(
            Guid.NewGuid(),
            productId,
            basePrice,
            now);

        price.RaiseDomainEvent(new PriceCreatedDomainEvent(vendorId, price));

        return Ok(price);
    }

    public IResult UpdateBasePrice(Money newPrice, DateTimeOffset now)
    {
        if (newPrice is null)
            return Fail(PricingErrors.BasePriceIsRequired());

        if (BasePrice == newPrice)
            return Ok();

        BasePrice = newPrice;

        var recalculateResult = RecalculateCurrentPrice(now);
        if (recalculateResult.IsFailure)
            return recalculateResult;

        UpdatedAt = now;

        RaiseDomainEvent(new BasePriceUpdatedDomainEvent(Id, ProductId, newPrice));

        return Ok();
    }

    public IResult SetDiscount(DiscountSnapshot discount, DateTimeOffset now)
    {
        if (discount is null)
            return Fail(PricingErrors.DiscountIsRequired());

        if (!discount.IsActive(now))
            return Fail(PricingErrors.DiscountIsRequired());

        if (AppliedDiscount is not null)
            return Fail(PricingErrors.DiscountAlreadySet());

        var discountedResult = discount.Apply(BasePrice, now);

        if (discountedResult.IsFailure)
            return discountedResult;

        if (discountedResult.Value >= BasePrice)
            return Fail(PricingErrors.InvalidDiscount());

        AppliedDiscount = discount;
        CurrentPrice = discountedResult.Value;

        UpdatedAt = now;

        RaiseDomainEvent(new DiscountAppliedDomainEvent(Id, discount.DiscountId));

        return Ok();
    }

    public IResult ReplaceDiscount(DiscountSnapshot discount, DateTimeOffset now)
    {
        if (discount is null)
            return Fail(PricingErrors.DiscountIsRequired());

        if (discount.IsExpired(now))
            return Fail(PricingErrors.InvalidDiscount());

        var discountedResult = discount.Apply(BasePrice, now);

        if (discountedResult.IsFailure)
            return discountedResult;

        if (discountedResult.Value >= BasePrice)
            return Fail(PricingErrors.InvalidDiscount());

        AppliedDiscount = discount;
        CurrentPrice = discountedResult.Value;

        UpdatedAt = now;

        RaiseDomainEvent(new DiscountAppliedDomainEvent(Id, discount.DiscountId));

        return Ok();
    }

    public IResult RemoveDiscount(DateTimeOffset now)
    {
        if (AppliedDiscount is null)
            return Ok();

        AppliedDiscount = null;
        CurrentPrice = BasePrice;

        UpdatedAt = now;

        RaiseDomainEvent(new DiscountRemovedDomainEvent(Id));

        return Ok();
    }

    public IResult<Money> GetCurrentPrice(DateTimeOffset now)
    {
        if (AppliedDiscount is null)
            return Ok(BasePrice);

        return AppliedDiscount.Apply(BasePrice, now);
    }

    public IResult RefreshPrice(DateTimeOffset now)
    {
        var result = RecalculateCurrentPrice(now);

        if (result.IsFailure)
            return result;

        UpdatedAt = now;
        
        RaiseDomainEvent(new PriceRefreshedDomainEvent(Id));

        return Ok();
    }

    private IResult RecalculateCurrentPrice(DateTimeOffset now)
    {
        if (AppliedDiscount is null)
        {
            CurrentPrice = BasePrice;
            return Ok();
        }

        if (AppliedDiscount.IsExpired(now))
        {
            AppliedDiscount = null;
            CurrentPrice = BasePrice;
            return Ok();
        }

        var discountedResult = AppliedDiscount.Apply(BasePrice, now);

        if (discountedResult.IsFailure)
            return discountedResult;

        CurrentPrice = discountedResult.Value;

        return Ok();
    }
}

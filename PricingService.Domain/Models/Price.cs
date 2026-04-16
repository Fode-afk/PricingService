using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
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
        CreatedAt = createdAt;
    }

    public Guid ProductId { get; private set; }

    public Money BasePrice { get; private set; }
    public DiscountSnapshot? Discount { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static IResult<Price> Create(
        Guid productId,
        Money basePrice,
        DateTimeOffset now)
    {
        var price = new Price(
            Guid.NewGuid(),
            productId,
            basePrice,
            now);

        return Ok(price);
    }

    public IResult UpdateBasePrice(Money newPrice, DateTimeOffset now)
    {
        if (newPrice is null)
            return Fail(PricingErrors.BasePriceIsRequired());

        if (BasePrice == newPrice)
            return Ok();

        if (Discount is not null)
        {
            var discounted = Discount.Apply(newPrice, now);

            if (discounted >= newPrice)
                return Fail(PricingErrors.InvalidDiscountForNewPrice());
        }

        BasePrice = newPrice;
        UpdatedAt = now;

        return Ok();
    }

    public IResult SetDiscount(DiscountSnapshot discount, DateTimeOffset now)
    {
        if (Discount != null)
            return Fail(PricingErrors.DiscountAlreadySet());

        if (discount is null)
            return Fail(PricingErrors.DiscountIsRequired());

        var discounted = discount.Apply(BasePrice, now);

        if (discounted >= BasePrice)
            return Fail(PricingErrors.InvalidDiscount());

        Discount = discount;
        UpdatedAt = now;

        return Ok();
    }

    public IResult RemoveDiscount(DateTimeOffset now)
    {
        if (Discount is null)
            return Ok();

        Discount = null;
        UpdatedAt = now;

        return Ok();
    }

    public Money GetCurrentPrice(DateTimeOffset now)
    {
        if (Discount is null)
            return BasePrice;

        return Discount.Apply(BasePrice, now);
    }
}

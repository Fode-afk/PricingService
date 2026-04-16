using migApp.Shared.Domain.Primitives;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Enums.Discounts;

namespace PricingService.Domain.ValueObjects;

public sealed class DiscountSnapshot : ValueObject
{
    private DiscountSnapshot(
        Guid discountId,
        DiscountType type,
        decimal? percentage,
        Money? fixedPrice,
        Money? amountOff,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        DiscountId = discountId;
        Type = type;
        Percentage = percentage;
        FixedPrice = fixedPrice;
        AmountOff = amountOff;
        Start = start;
        End = end;
    }

    public Guid DiscountId { get; }
    public DiscountType Type { get; }
    public decimal? Percentage { get; }
    public Money? FixedPrice { get; }
    public Money? AmountOff { get; }

    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }

    public bool IsActive(DateTimeOffset now)
        => now >= Start && now < End;

    public Money Apply(Money basePrice, DateTimeOffset now)
    {
        if (!IsActive(now))
            return basePrice;

        return Type switch
        {
            DiscountType.Percentage => ApplyPercentage(basePrice),
            DiscountType.FixedPrice => FixedPrice!,
            DiscountType.AmountOff => basePrice - AmountOff!,
            _ => basePrice
        };
    }

    private Money ApplyPercentage(Money basePrice)
    {
        var discountAmount = basePrice.Amount * (Percentage!.Value / 100m);
        return Money.Create(basePrice.Amount - discountAmount, basePrice.Currency).Value;
    }

    public static DiscountSnapshot CreatePercentage(
        Guid discountId,
        decimal percentage,
        DateTimeOffset start,
        DateTimeOffset end)
        => new(discountId, DiscountType.Percentage, percentage, null, null, start, end);

    public static DiscountSnapshot CreateFixed(
        Guid discountId,
        Money price,
        DateTimeOffset start,
        DateTimeOffset end)
        => new(discountId, DiscountType.FixedPrice, null, price, null, start, end);

    public static DiscountSnapshot CreateAmountOff(
        Guid discountId,
        Money amountOff,
        DateTimeOffset start,
        DateTimeOffset end)
        => new(discountId, DiscountType.AmountOff, null, null, amountOff, start, end);

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return DiscountId;
        yield return Type;
        yield return Percentage;
        yield return FixedPrice;
        yield return AmountOff;
        yield return Start;
        yield return End;
    }
}

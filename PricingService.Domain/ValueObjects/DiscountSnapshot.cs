using migApp.Shared.Domain.Primitives;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Enums.Discounts;
using migApp.Shared.Results;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.ValueObjects;

public sealed class DiscountSnapshot : ValueObject
{
    private DiscountSnapshot(
        Guid discountId,
        DiscountType type,
        decimal? percentage,
        Money? fixedPrice,
        Money? amountOff,
        CampaignName? campaignName,
        DiscountPriority priority,
        bool isStackable,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        DiscountId = discountId;
        Type = type;
        Percentage = percentage;
        FixedPrice = fixedPrice;
        AmountOff = amountOff;
        CampaignName = campaignName;
        Priority = priority;
        IsStackable = isStackable;
        Start = start;
        End = end;
    }

    public Guid DiscountId { get; }
    public DiscountType Type { get; }
    public decimal? Percentage { get; }
    public Money? FixedPrice { get; }
    public Money? AmountOff { get; }

    public CampaignName? CampaignName { get; }
    public bool HasCampaign => CampaignName is not null;
    public DiscountPriority Priority { get; }
    public bool IsStackable { get; }

    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }

    public TimeSpan Duration => End - Start;

    public bool IsActive(DateTimeOffset now)
        => now >= Start && now < End;

    public bool IsExpired(DateTimeOffset now)
        => now >= End;

    public IResult<Money> Apply(Money basePrice, DateTimeOffset now)
    {
        if (!IsActive(now))
            return Ok(basePrice);

        return Type switch
        {
            DiscountType.Percentage => ApplyPercentage(basePrice),
            DiscountType.FixedPrice => ApplyFixedPrice(basePrice),
            DiscountType.AmountOff => ApplyAmountOff(basePrice),
            _ => Ok(basePrice)
        };
    }

    private IResult<Money> ApplyPercentage(Money basePrice)
    {
        var discountAmount = basePrice.Amount * (Percentage!.Value / 100m);
        return Money.Create(basePrice.Amount - discountAmount, basePrice.Currency);
    }

    private IResult<Money> ApplyFixedPrice(Money basePrice)
    {
        if (FixedPrice >= basePrice)
            return Fail<Money>(DiscountSnapshotErrors.InvalidFixedPrice());

        return Ok(FixedPrice!);
    }

    private IResult<Money> ApplyAmountOff(Money basePrice)
    {
        var discounted = basePrice - AmountOff!;

        if (discounted.Amount < 0)
            return Ok(Money.Zero(basePrice.Currency));

        return Ok(discounted);
    }

    public static IResult<Money> ApplyDiscounts(
        Money basePrice,
        IEnumerable<DiscountSnapshot> discounts,
        DateTimeOffset now)
    {
        var active = discounts
            .Where(x => x.IsActive(now))
            .OrderByDescending(x => x.Priority.Value)
            .ToList();

        var price = basePrice;

        bool discountApplied = false;

        foreach (var discount in active)
        {
            if (discountApplied && !discount.IsStackable)
                break;

            var applyResult = discount.Apply(price, now);
            if (applyResult.IsFailure)
                return applyResult;

            price = applyResult.Value;
            discountApplied = true;

            if (!discount.IsStackable)
                break;
        }

        return Ok(price);
    }

    public static IResult<DiscountSnapshot> CreatePercentage(
        Guid discountId,
        decimal percentage,
        CampaignName? campaignName,
        DiscountPriority priority,
        bool isStackable,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        if (percentage > 100 || percentage < 1)
            return Fail<DiscountSnapshot>(DiscountSnapshotErrors.InvalidPercentage());

        if (end <= start)
            return Fail<DiscountSnapshot>(DiscountSnapshotErrors.InvalidDateRange());

        return Ok(
            new DiscountSnapshot(   
                discountId,
                DiscountType.Percentage,
                percentage,
                null,
                null,
                campaignName,
                priority,
                isStackable,
                start,
                end));
    }

    public static IResult<DiscountSnapshot> CreateFixed(
        Guid discountId,
        Money price,
        CampaignName? campaignName,
        DiscountPriority priority,
        bool isStackable,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        if (end <= start)
            return Fail<DiscountSnapshot>(DiscountSnapshotErrors.InvalidDateRange());

        return Ok(
            new DiscountSnapshot(
                discountId,
                DiscountType.FixedPrice,
                null,
                price,
                null,
                campaignName,
                priority,
                isStackable,
                start,
                end));
    }

    public static IResult<DiscountSnapshot> CreateAmountOff(
        Guid discountId,
        Money amountOff,
        CampaignName? campaignName,
        DiscountPriority priority,
        bool isStackable,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        if (end <= start)
            return Fail<DiscountSnapshot>(DiscountSnapshotErrors.InvalidDateRange());

        return Ok(
            new DiscountSnapshot(
                discountId,
                DiscountType.AmountOff,
                null,
                null,
                amountOff,
                campaignName,
                priority,
                isStackable,
                start,
                end));
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return DiscountId;
        yield return Type;
        yield return Percentage;
        yield return FixedPrice;
        yield return AmountOff;
        yield return CampaignName;
        yield return Priority;
        yield return IsStackable;
        yield return Start;
        yield return End;
    }
}

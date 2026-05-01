using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.ValueObjects;

public sealed class DiscountPriority : ValueObject
{
    private DiscountPriority(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static DiscountPriority Highest => new(100);
    public static DiscountPriority Default => new(1);

    public static IResult<DiscountPriority> Create(int value)
    {
        if (value < 1)
            return Fail<DiscountPriority>(DiscountPriorityErrors.InvalidPriority());

        if (value > 1000)
            return Fail<DiscountPriority>(DiscountPriorityErrors.InvalidPriority());

        return Ok(new DiscountPriority(value));
    }

    public bool IsHigherThan(DiscountPriority other)
        => Value > other.Value;

    public bool IsLowerThan(DiscountPriority other)
        => Value < other.Value;

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString()
        => Value.ToString();

    public static implicit operator int(DiscountPriority priority)
        => priority.Value;
}

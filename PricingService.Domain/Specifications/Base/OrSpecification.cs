using migApp.Shared.Results;

namespace PricingService.Domain.Specifications.Base;

public sealed class OrSpecification<T>(
    ISpecification<T> left,
    ISpecification<T> right) : Specification<T>
{
    public override IResult IsSatisfiedBy(T candidate)
    {
        var leftResult = left.IsSatisfiedBy(candidate);
        if (leftResult.IsSuccess)
            return leftResult;

        return right.IsSatisfiedBy(candidate);
    }
}

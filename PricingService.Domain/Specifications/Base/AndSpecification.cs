using migApp.Shared.Results;

namespace PricingService.Domain.Specifications.Base;

public sealed class AndSpecification<T>(
    ISpecification<T> left,
    ISpecification<T> right) : Specification<T>
{
    public override IResult IsSatisfiedBy(T candidate)
    {
        var leftResult = left.IsSatisfiedBy(candidate);
        if (leftResult.IsFailure)
            return leftResult;

        var rightResult = right.IsSatisfiedBy(candidate);
        if (rightResult.IsFailure)
            return rightResult;

        return ResultFactory.Ok();
    }
}
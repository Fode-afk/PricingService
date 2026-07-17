using migApp.Shared.Results;

namespace PricingService.Domain.Specifications.Base;

public sealed class NotSpecification<T>(
    ISpecification<T> inner) : Specification<T>
{
    public override IResult IsSatisfiedBy(T candidate)
    {
        var result = inner.IsSatisfiedBy(candidate);

        return result.IsSuccess
            ? ResultFactory.Fail(Error.InvalidArgument("NOT_SPEC_FAILED",
                "The specified condition is not satisfied."))
            : ResultFactory.Ok();
    }
}

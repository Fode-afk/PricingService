using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Specifications.Base;

internal sealed class LambdaSpecification<T>(
    Func<T, bool> predicate,
    Error error) : Specification<T>
{
    public override IResult IsSatisfiedBy(T ctx)
        => predicate(ctx) ? Ok() : Fail(error);
}
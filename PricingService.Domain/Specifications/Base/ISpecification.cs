using migApp.Shared.Results;

namespace PricingService.Domain.Specifications.Base;

public interface ISpecification<T>
{
    IResult IsSatisfiedBy(T candidate);
}
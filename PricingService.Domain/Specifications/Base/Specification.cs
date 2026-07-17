using migApp.Shared.Results;

namespace PricingService.Domain.Specifications.Base;

public abstract class Specification<T> : ISpecification<T>
{
    public abstract IResult IsSatisfiedBy(T candidate);

    public static Specification<T> Create(
        Func<T, bool> predicate,
        Error error)
        => new LambdaSpecification<T>(predicate, error);

    public Specification<T> And(Specification<T> other)
        => new AndSpecification<T>(this, other);

    public Specification<T> Or(Specification<T> other)
        => new OrSpecification<T>(this, other);

    public Specification<T> Not()
        => new NotSpecification<T>(this);
}
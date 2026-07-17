using migApp.Shared.Results;
using PricingService.Domain.Abstractions;
using PricingService.Domain.Errors;
using PricingService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Specifications.Common;

public sealed class ProductCanBeModifiedSpec<T> : Specification<T>
    where T : IProductContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.ProductCanBeModified)
            return Fail(ProductSnapshotErrors.CannotModify());

        return Ok();
    }
}
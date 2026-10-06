using migApp.Shared.Results;
using PricingService.Domain.Abstractions;
using PricingService.Domain.Errors;
using PricingService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Specifications.Common;

public sealed class ProductCanEditOperationalDataSpec<T> : Specification<T>
    where T : IProductContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.ProductCanEditOperationalData)
            return Fail(ProductSnapshotErrors.CannotEditOperationalData());

        return Ok();
    }
}
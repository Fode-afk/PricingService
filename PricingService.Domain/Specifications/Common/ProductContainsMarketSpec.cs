using migApp.Shared.Results;
using PricingService.Domain.Abstractions;
using PricingService.Domain.Errors;
using PricingService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Specifications.Common;

internal sealed class ProductContainsMarketSpec<T> : Specification<T>
    where T : IProductContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.ProductContainsMarket)
            return Fail(ProductSnapshotErrors.MarketNotFound());

        return Ok();
    }
}

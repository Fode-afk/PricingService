using PricingService.Domain.Context;
using PricingService.Domain.Specifications.Base;
using PricingService.Domain.Specifications.Common;

namespace PricingService.Domain.Specifications.Price;

public static class PriceCreationSpecification
{
    public static readonly ISpecification<PriceCreationContext> Spec =
        new VendorIsActiveSpec<PriceCreationContext>()
            .And(new ProductCanBeModifiedSpec<PriceCreationContext>());
}

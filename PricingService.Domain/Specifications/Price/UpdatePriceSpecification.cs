using PricingService.Domain.Context;
using PricingService.Domain.Specifications.Base;
using PricingService.Domain.Specifications.Common;

namespace PricingService.Domain.Specifications.Price;

public static class UpdatePriceSpecification
{
    public static readonly ISpecification<UpdatePriceContext> Spec =
        new VendorIsActiveSpec<UpdatePriceContext>()
            .And(new ProductCanEditOperationalDataSpec<UpdatePriceContext>());
}

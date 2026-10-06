using PricingService.Domain.Abstractions;

namespace PricingService.Domain.Context;

public sealed record UpdatePriceContext(
    bool VendorIsActive,
    bool ProductCanEditOperationalData) : IVendorContext, IProductContext;
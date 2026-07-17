using PricingService.Domain.Abstractions;

namespace PricingService.Domain.Context;

public sealed record UpdatePriceContext(
    bool VendorIsActive,
    bool ProductCanBeModified) : IVendorContext, IProductContext;
using PricingService.Domain.Abstractions;

namespace PricingService.Domain.Context;

public sealed record PriceCreationContext(
    bool VendorIsActive,
    bool ProductCanBeModified) : IVendorContext, IProductContext;
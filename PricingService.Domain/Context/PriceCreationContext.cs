using PricingService.Domain.Abstractions;

namespace PricingService.Domain.Context;

public sealed record PriceCreationContext(
    bool VendorIsActive,
    bool ProductCanEditOperationalData) : IVendorContext, IProductContext;
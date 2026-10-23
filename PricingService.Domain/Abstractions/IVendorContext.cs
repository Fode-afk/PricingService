namespace PricingService.Domain.Abstractions;

internal interface IVendorContext
{
    bool VendorIsActive { get; }
}

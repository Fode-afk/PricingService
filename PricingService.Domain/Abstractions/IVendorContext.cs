namespace PricingService.Domain.Abstractions;

public interface IVendorContext
{
    bool VendorIsActive { get; }
}

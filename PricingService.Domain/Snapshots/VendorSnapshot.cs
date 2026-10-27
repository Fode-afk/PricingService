using migApp.Shared.Domain.ValueObjects;

namespace PricingService.Domain.Snapshots;

public sealed class VendorSnapshot
{
    public Guid VendorId { get; init; }
    public required Currency BaseCurrency { get; init; } 
    public bool IsActive { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}
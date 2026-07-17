namespace PricingService.Domain.Snapshots;

public sealed class VendorSnapshot
{
    public Guid VendorId { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public long Version { get; set; }
}
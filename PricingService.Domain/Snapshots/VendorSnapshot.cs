namespace PricingService.Domain.Snapshots;

public sealed class VendorSnapshot
{
    public Guid VendorId { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public long Version { get; set; }
}
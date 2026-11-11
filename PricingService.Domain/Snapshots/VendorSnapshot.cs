namespace PricingService.Domain.Snapshots;

public sealed class VendorSnapshot
{
    public Guid VendorId { get; init; }
    public bool IsActive { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}
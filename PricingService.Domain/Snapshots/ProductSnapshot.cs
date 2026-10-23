namespace PricingService.Domain.Snapshots;

public sealed class ProductSnapshot
{
    public Guid ProductId { get; init; }
    public Guid VendorId { get; init; }
    public bool CanEditOperationalData { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}
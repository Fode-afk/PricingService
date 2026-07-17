namespace PricingService.Domain.Snapshots;

public sealed class ProductVariantSnapshot
{
    public Guid ProductVariantId { get; set; }
    public Guid ProductId { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}
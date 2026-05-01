namespace PricingService.Domain.Models;

public sealed class ProductSnapshot
{
    public Guid ProductId { get; set; }
    public Guid VendorId { get; set; }
    public Guid ProductCardId { get; set; }
    public ProductCardStatus Status { get; set; }
}

public enum ProductCardStatus
{
    Draft,
    Published,
    Archived
}
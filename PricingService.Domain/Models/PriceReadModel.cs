namespace PricingService.Domain.Models;

public sealed class PriceReadModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }

    public Guid? DiscountId { get; set; }
    public int? DiscountType { get; set; }

    public decimal? Percentage { get; set; }
    public decimal? FixedPrice { get; set; }
    public decimal? AmountOff { get; set; }

    public string? CampaignName { get; set; }

    public int? Priority { get; set; }
    public bool? IsStackable { get; set; }

    public DateTimeOffset? DiscountStart { get; set; }
    public DateTimeOffset? DiscountEnd { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

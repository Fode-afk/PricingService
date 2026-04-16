namespace PricingService.Application.Caching;

public static class CacheTags
{
    public static string PriceByProductId(Guid productId) => $"PriceByProductId:{productId}";
}
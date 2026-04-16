namespace PricingService.Application.Caching;

public static class CacheKeys
{
    public static string PriceByProductId(Guid productId, string currencyCode) => $"Price:{productId}:{currencyCode}";
    public static string ExchangeRateByCurrency(string currency) => $"ExchangeRate:{currency}";
}

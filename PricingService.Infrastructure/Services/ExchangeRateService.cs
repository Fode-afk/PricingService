using migApp.Shared.Domain.ValueObjects;
using PricingService.Application.Interfaces.Services;
using ZiggyCreatures.Caching.Fusion;

namespace PricingService.Infrastructure.Services;

internal sealed class ExchangeRateService(IFusionCache cache, ICurrencyService currencyService) : IExchangeRateService
{
    private static string ExchangeRateByCurrency(string currency) => $"ExchangeRate:{currency}";

    public async Task<decimal?> GetExchangeRateAsync(Currency sourceCurrency, Currency targetCurrency, CancellationToken cancellationToken = default)
    {
        if (sourceCurrency == targetCurrency)
            return 1m;

        var sourceCurrencyRate = await cache.GetOrDefaultAsync<decimal?>(
            ExchangeRateByCurrency(sourceCurrency.Code),
            options: new FusionCacheEntryOptions { Duration = TimeSpan.FromHours(1) },
            token: cancellationToken);

        var targetCurrencyRate = await cache.GetOrDefaultAsync<decimal?>(
            ExchangeRateByCurrency(targetCurrency.Code),
            options: new FusionCacheEntryOptions { Duration = TimeSpan.FromHours(1) },
            token: cancellationToken);

        if (sourceCurrencyRate is null || targetCurrencyRate is null || sourceCurrencyRate == 0)
            return await currencyService.GetExchangeRateAsync(
                sourceCurrency.Code,
                targetCurrency.Code,
                cancellationToken);

        return targetCurrencyRate.Value / sourceCurrencyRate.Value;
    }

    public async Task UpdateExcahngeRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken = default)
    {
        var tasks = rates.Select(async rate =>
            await cache.SetAsync(
                    ExchangeRateByCurrency(rate.Key),
                    rate.Value,
                    options: new FusionCacheEntryOptions { Duration = TimeSpan.FromHours(1) },
                    token: cancellationToken));

        await Task.WhenAll(tasks);
    }
}
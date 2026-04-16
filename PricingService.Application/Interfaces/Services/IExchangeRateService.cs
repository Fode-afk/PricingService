using migApp.Shared.Domain.ValueObjects;

namespace PricingService.Application.Interfaces.Services;

public interface IExchangeRateService
{
    Task<decimal?> GetExchangeRateAsync(
        Currency sourceCurrency,
        Currency targetCurrency,
        CancellationToken cancellationToken = default);

    Task UpdateExcahngeRatesAsync(
        Dictionary<string, decimal> rates,
        CancellationToken cancellationToken = default);
}

using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Services;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Infrastructure.Services;

internal sealed class MoneyConverter(IExchangeRateService exchangeRateService) : IMoneyConverter
{
    public async Task<IResult<Money>> ConvertAsync(Money money, Currency targetCurrency, CancellationToken cancellationToken = default)
    {
        if (money.Currency == targetCurrency)
            return Ok(money);

        var exchangeRate = await exchangeRateService.GetExchangeRateAsync(money.Currency, targetCurrency, cancellationToken);

        if (exchangeRate == null)
            return Fail<Money>(MoneyErrors.ExchangeRateNotFound());      

        var convertedAmount = Math.Round(
            money.Amount * exchangeRate.Value,
            targetCurrency.MinorUnits,
            MidpointRounding.ToEven);

        var result = Money.Create(convertedAmount, targetCurrency);

        if (result.IsFailure)
            return result;

        return Ok(result.Value);
    }
}

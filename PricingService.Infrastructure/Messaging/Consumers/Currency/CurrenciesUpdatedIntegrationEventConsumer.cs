using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ExchangeRates;
using PricingService.Application.Interfaces.Services;

namespace PricingService.Infrastructure.Messaging.Consumers.Currency;

public sealed class CurrenciesUpdatedIntegrationEventConsumer(IExchangeRateService exchangeRateService) : IConsumer<CurrenciesUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CurrenciesUpdatedIntegrationEvent> context) =>
        await exchangeRateService.UpdateExcahngeRatesAsync(context.Message.Rates, context.CancellationToken);
}
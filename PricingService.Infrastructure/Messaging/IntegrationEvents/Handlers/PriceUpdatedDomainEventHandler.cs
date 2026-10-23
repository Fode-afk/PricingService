using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;
using PricingService.Domain.DomainEvents;

namespace PricingService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class PriceUpdatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<PriceUpdatedDomainEvent>
{
    public async Task Handle(PriceUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new PriceUpdatedIntegrationEvent(
            notification.ProductVariantId,
            notification.CurrentPrice.ToMinor(),
            notification.CurrentPrice.Currency,
            notification.Version), cancellationToken);
}
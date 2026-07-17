using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Primitives;

namespace PricingService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class PriceUpdatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<PriceUpdatedDomainEvent>
{
    public async Task Handle(PriceUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new PriceUpdatedIntegrationEvent(
            notification.ProductVariantId,
            notification.HasActivePrice,
            notification.Version), cancellationToken);
}
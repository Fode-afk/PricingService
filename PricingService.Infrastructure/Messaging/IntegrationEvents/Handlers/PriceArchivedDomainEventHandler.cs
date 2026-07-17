using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Primitives;

namespace PricingService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class PriceArchivedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<PriceArchivedDomainEvent>
{
    public async Task Handle(PriceArchivedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new PriceArchivedIntegrationEvent(notification.ProductVariantId), cancellationToken);
}

using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Primitives;

namespace PricingService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class PriceCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<PriceCreatedDomainEvent>
{
    public async Task Handle(PriceCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new PriceCreatedIntegrationEvent(
            notification.Price.ProductId,
            notification.VendorId,
            notification.Price.CurrentPrice.Amount), cancellationToken);
}

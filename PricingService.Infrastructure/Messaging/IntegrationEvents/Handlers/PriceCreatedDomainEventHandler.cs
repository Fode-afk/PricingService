using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;
using PricingService.Domain.DomainEvents;

namespace PricingService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class PriceCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<PriceCreatedDomainEvent>
{
    public async Task Handle(PriceCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new PriceCreatedIntegrationEvent(
            notification.PriceId,
            notification.ProductVariantId,
            notification.IsBase,
            notification.CurrentPrice.ToMinor(),
            notification.CurrentPrice.Currency,
            notification.Version), cancellationToken);
}
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshotMarkets;

namespace PricingService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductMarketsChangedIntegrationEventConsumer(ISender sender) : IConsumer<ProductMarketsChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductMarketsChangedIntegrationEvent> context) =>
        await sender.Send(new UpdateProductSnapshotMarketsCommand(
            context.Message.ProductId,
            context.Message.Markets,
            context.Message.RemovedMarkets,
            context.Message.Version), context.CancellationToken);
}
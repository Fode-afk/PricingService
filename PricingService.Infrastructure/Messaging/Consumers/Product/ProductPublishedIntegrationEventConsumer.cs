using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductPublishedIntegrationEventConsumer(ISender sender) : IConsumer<ProductPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPublishedIntegrationEvent> context) => 
        await sender.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CanEditOperationalData,
            context.Message.Version), context.CancellationToken);
}

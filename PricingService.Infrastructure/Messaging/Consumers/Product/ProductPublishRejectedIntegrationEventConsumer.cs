using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductPublishRejectedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductPublishRejectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPublishRejectedIntegrationEvent> context) =>
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CanEditOperationalData,
            context.Message.Version), context.CancellationToken);
}
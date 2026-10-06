using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductUnsuspendedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductUnsuspendedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductUnsuspendedIntegrationEvent> context) =>
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CanEditOperationalData,
            context.Message.Version), context.CancellationToken);
}
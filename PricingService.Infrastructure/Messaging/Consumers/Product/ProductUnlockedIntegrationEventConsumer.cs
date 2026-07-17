using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductUnlockedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductUnlockedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductUnlockedIntegrationEvent> context) =>
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CanBeModified,
            context.Message.Version), context.CancellationToken);
}

using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using PricingService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.ProductVariant;

public sealed class ProductVariantDeletedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductVariantDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductVariantDeletedIntegrationEvent> context) => 
        await mediator.Send(new DeleteProductVariantSnapshotCommand(context.Message.ProductVariantId), context.CancellationToken);
}

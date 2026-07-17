using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using PricingService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.ProductVariant;

public sealed class ProductVariantCreatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductVariantCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductVariantCreatedIntegrationEvent> context) =>
        await mediator.Send(new AddProductVariantSnapshotCommand(
            context.Message.ProductVariantId,
            context.Message.ProductId,
            context.Message.Version), context.CancellationToken);
}
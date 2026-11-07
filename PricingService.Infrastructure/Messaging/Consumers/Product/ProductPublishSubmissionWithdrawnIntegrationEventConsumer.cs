using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace PricingService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductPublishSubmissionWithdrawnIntegrationEventConsumer(ISender sender) : IConsumer<ProductPublishSubmissionWithdrawnIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPublishSubmissionWithdrawnIntegrationEvent> context) =>
        await sender.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CanEditOperationalData,
            context.Message.Version), context.CancellationToken);
}

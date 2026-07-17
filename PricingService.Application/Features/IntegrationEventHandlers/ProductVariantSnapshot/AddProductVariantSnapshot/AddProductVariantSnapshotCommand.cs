using MediatR;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;

public sealed record AddProductVariantSnapshotCommand(
    Guid ProductVariantId,
    Guid ProductId,
    long Version) : IRequest;

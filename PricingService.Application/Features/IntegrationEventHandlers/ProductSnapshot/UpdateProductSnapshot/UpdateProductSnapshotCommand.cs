using MediatR;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

public sealed record UpdateProductSnapshotCommand(
    Guid ProductId,
    bool CanBeModified,
    long Version) : IRequest;
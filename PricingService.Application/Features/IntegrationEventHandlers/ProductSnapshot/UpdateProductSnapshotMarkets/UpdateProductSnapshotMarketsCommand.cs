using MediatR;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshotMarkets;

public sealed record UpdateProductSnapshotMarketsCommand(
    Guid ProductId,
    IReadOnlyList<string> Markets,
    IReadOnlyCollection<string> RemovedMarkets,
    long Version) : IRequest;
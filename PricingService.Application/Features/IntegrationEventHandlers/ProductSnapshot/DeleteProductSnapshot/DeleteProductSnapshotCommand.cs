using MediatR;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.DeleteProductSnapshot;

public sealed record DeleteProductSnapshotCommand(Guid ProductId) : IRequest;
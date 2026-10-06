using MediatR;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.AddProductSnapshot;

public sealed record AddProductSnapshotCommand(
    Guid ProductId,
    Guid VendorId,
    bool CanEditOperationalData,
    long Version) : IRequest;
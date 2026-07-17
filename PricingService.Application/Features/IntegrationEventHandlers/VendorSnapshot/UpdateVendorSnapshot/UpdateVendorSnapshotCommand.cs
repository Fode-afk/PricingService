using MediatR;

namespace PricingService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed record UpdateVendorSnapshotCommand(
    Guid VendorId,
    bool IsActive,
    long Version) : IRequest;
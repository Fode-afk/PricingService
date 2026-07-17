using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed record DeleteVendorSnapshotCommand(Guid VendorId) : IRequest;
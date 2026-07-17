using MediatR;
using Microsoft.EntityFrameworkCore;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Metrics;
using PricingService.Domain.Exceptions;

namespace PricingService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed class UpdateVendorSnapshotCommandHandler(
    IAppDbContext context,
    IPricingMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateVendorSnapshotCommand>
{
    public async Task Handle(UpdateVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);

        if (snapshot is null)
        {
            metrics.RecordSnapshotNotFound("Vendor");
            throw new SnapshotNotFoundException("Vendor", request.VendorId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateVendorSnapshotCommand));
            return;
        }

        snapshot.IsActive = request.IsActive;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);
    }
}

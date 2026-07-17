using MediatR;
using Microsoft.EntityFrameworkCore;
using PricingService.Application.Interfaces.Data;

namespace PricingService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;

public sealed class AddVendorSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddVendorSnapshotCommand>
{
    public async Task Handle(AddVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.VendorSnapshots
            .AnyAsync(x => x.VendorId == request.VendorId, cancellationToken);
        if (exists)
            return;

        context.VendorSnapshots.Add(
            new Domain.Snapshots.VendorSnapshot
            {
                VendorId = request.VendorId,
                IsActive = request.IsActive,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Metrics;
using PricingService.Domain.Exceptions;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

public sealed class UpdateProductSnapshotCommandHandler(
    IAppDbContext context,
    IPricingMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductSnapshotCommand>
{
    public async Task Handle(UpdateProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductSnapshots
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

        if (snapshot is null)
        {
            metrics.RecordSnapshotNotFound("Product");
            throw new SnapshotNotFoundException("Product", request.ProductId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateProductSnapshotCommand));
            return;
        }

        snapshot.CanBeModified = request.CanBeModified;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);
    }
}
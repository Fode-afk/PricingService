using MediatR;
using Microsoft.EntityFrameworkCore;
using PricingService.Application.Interfaces.Data;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;

public sealed class AddProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductVariantSnapshotCommand>
{
    public async Task Handle(AddProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductVariantSnapshots
            .AnyAsync(x => x.ProductVariantId == request.ProductVariantId, cancellationToken);

        if (exists)
            return;

        context.ProductVariantSnapshots.Add(
            new Domain.Snapshots.ProductVariantSnapshot
            {
                ProductVariantId = request.ProductVariantId,
                ProductId = request.ProductId,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
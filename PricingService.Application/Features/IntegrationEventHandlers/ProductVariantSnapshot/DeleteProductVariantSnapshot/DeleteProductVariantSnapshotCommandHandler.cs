using CatalogService.Application.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Metrics;
using PricingService.Domain.Exceptions;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;

public sealed class DeleteProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    IPricingMetrics metrics,
    ILogger<DeleteProductVariantSnapshotCommandHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<DeleteProductVariantSnapshotCommand>
{
    public async Task Handle(DeleteProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantSnapshots
            .FirstOrDefaultAsync(v => v.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return;

        var price = await context.Prices
            .FirstOrDefaultAsync(p => p.ProductVariantId == snapshot.ProductVariantId, cancellationToken);

        if (price is null)
        {
            metrics.RecordPriceNotFound();
            throw new PriceNotFoundException(snapshot.ProductVariantId);
        }

        var result = price.Archive(timeProvider.GetUtcNow());

        if (result.IsFailure)
            logger.PriceArchiveFailed(price.Id, result.Error.Code);

        context.ProductVariantSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Metrics;
using PricingService.Domain.Exceptions;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshotMarkets;

public sealed class UpdateProductSnapshotMarketsCommandHandler(
    IAppDbContext context,
    IPricingMetrics metrics,
    ILogger<UpdateProductSnapshotMarketsCommandHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductSnapshotMarketsCommand>
{
    public async Task Handle(UpdateProductSnapshotMarketsCommand request, CancellationToken cancellationToken)
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

        List<Market> markets = [];

        foreach (var marketCode in request.Markets)
        {
            var marketResult = Market.Create(marketCode);

            if (marketResult.IsFailure)
            {
                logger.LogWarning(
                   "Failed to create Market from code {MarketCode}. Error: {Error}",
                   marketCode,
                   marketResult.Error);
                return;
            }

            markets.Add(marketResult.Value);
        }

        List<Market> removed = [];
        foreach (var marketCode in request.RemovedMarkets)
        {
            var marketResult = Market.Create(marketCode);

            if (marketResult.IsFailure)
            {
                logger.LogWarning(
                   "Failed to create Market from code {MarketCode}. Error: {Error}",
                   marketCode,
                   marketResult.Error);
                return;
            }

            removed.Add(marketResult.Value);
        }

        var now = timeProvider.GetUtcNow();

        snapshot.Markets = markets;
        snapshot.UpdatedAt = now;
        snapshot.Version = request.Version;

        if (removed.Count > 0)
        {
            var variantIds = await context.ProductVariantSnapshots
                .Where(v => v.ProductId == request.ProductId)
                .Select(v => v.ProductVariantId)
                .ToListAsync(cancellationToken);

            var prices = await context.Prices
                .Include(p => p.Entries)
                .Include(p => p.History)
                .Where(p => variantIds.Contains(p.ProductVariantId))
                .ToListAsync(cancellationToken);

            foreach (var price in prices.Where(p => removed.Contains(p.Market)))
            {
                var result = price.Archive(now);
                if (result.IsFailure)
                    logger.LogWarning(
                        "Failed to archive price {PriceId} for removed market {Market}: {Error}",
                        price.Id, price.Market, result.Error);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

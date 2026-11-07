using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Application.Interfaces.Data;

namespace PricingService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;

public sealed class AddVendorSnapshotCommandHandler(
    IAppDbContext context,
    ILogger<AddVendorSnapshotCommandHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<AddVendorSnapshotCommand>
{
    public async Task Handle(AddVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.VendorSnapshots
            .AnyAsync(x => x.VendorId == request.VendorId, cancellationToken);

        if (exists)
            return;

        var baseCurrencyResult = Currency.Create(request.BaseCurrency);

        if (baseCurrencyResult.IsFailure)
        {
            logger.LogWarning(
                "Failed to create vendor snapshot due to invalid base currency. " +
                "VendorId: {VendorId}, BaseCurrency: {BaseCurrency}, Error: {Error}",
                request.VendorId,
                request.BaseCurrency,
                baseCurrencyResult.Error);
            return;
        }
        
        context.VendorSnapshots.Add(
            new Domain.Snapshots.VendorSnapshot
            {
                VendorId = request.VendorId,
                BaseCurrency = baseCurrencyResult.Value,
                IsActive = request.IsActive,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
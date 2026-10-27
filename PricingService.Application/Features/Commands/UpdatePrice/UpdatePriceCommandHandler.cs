using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Domain.Context;
using PricingService.Domain.Enums;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.UpdatePrice;

public sealed class UpdatePriceCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdatePriceCommand, IResult>
{
    public async Task<IResult> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
    {
        var vendorSnapshot = await context.VendorSnapshots
           .AsNoTracking()
           .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

        var variantSnapshot = await context.ProductVariantSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(pv => pv.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (variantSnapshot is null)
            return Fail(ProductVariantSnapshotErrors.NotFound());

        var productSnapshot = await context.ProductSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == variantSnapshot.ProductId, cancellationToken);
        if (productSnapshot is null)
            return Fail(ProductSnapshotErrors.NotFound());

        if (productSnapshot.VendorId != request.VendorId)
            return Fail(ProductSnapshotErrors.DoesNotBelongToVendor());

        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return currencyResult;

        var price = await context.Prices
            .Include(p => p.Entries)
            .Include(p => p.History)
            .FirstOrDefaultAsync(p =>
                p.VendorId == vendorSnapshot.VendorId &&
                p.ProductVariantId == request.ProductVariantId &&
                p.Currency == currencyResult.Value, cancellationToken);
        if (price is null)
            return Fail(PriceErrors.NotFound());

        var newPriceResult = Money.FromMinor(request.NewPriceMinor, currencyResult.Value);
        if (newPriceResult.IsFailure)
            return newPriceResult;

        var ctx = new UpdatePriceContext(vendorSnapshot.IsActive, productSnapshot.CanEditOperationalData);

        var result = price.UpdatePrice(
            ctx,
            newPriceResult.Value, 
            request.VendorId,
            PriceChangeReason.ManualUpdate,
            $"vendor:{request.VendorId}",
            timeProvider.GetUtcNow(),
            request.EffectiveFrom,
            request.EffectiveTo);
        if (result.IsFailure)
            return Fail(result.Error);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
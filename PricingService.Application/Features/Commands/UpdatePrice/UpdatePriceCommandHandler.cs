using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Context;
using PricingService.Domain.Enums;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.UpdatePrice;

public sealed class UpdatePriceCommandHandler(
    IAppDbContext context,
    IMoneyConverter moneyConverter,
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

        var price = await context.Prices
            .FirstOrDefaultAsync(p => p.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (price is null)
            return Fail(PriceErrors.NotFound());

        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return currencyResult;

        var newPriceResult = Money.FromMinor(request.NewPriceMinor, currencyResult.Value);
        if (newPriceResult.IsFailure)
            return newPriceResult;

        var newPriceInUsdResult = await moneyConverter.ConvertAsync(
            newPriceResult.Value,
            Currency.USD,
            cancellationToken);
        if (newPriceInUsdResult.IsFailure)
            return newPriceInUsdResult;

        var ctx = new UpdatePriceContext(vendorSnapshot.IsActive, productSnapshot.CanEditOperationalData);

        var result = price.UpdatePrice(
            ctx,
            newPriceInUsdResult.Value, 
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

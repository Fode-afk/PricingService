using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Domain.Context;
using PricingService.Domain.Errors;
using PricingService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.CreatePrice;

public sealed class CreatePriceCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<CreatePriceCommand, IResult>
{
    public async Task<IResult> Handle(CreatePriceCommand request, CancellationToken cancellationToken)
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

        var basePriceResult = Money.FromMinor(request.BasePriceMinor, currencyResult.Value);  
        if (basePriceResult.IsFailure)
            return basePriceResult;

        var ctx = new PriceCreationContext(
            vendorSnapshot.IsActive,
            productSnapshot.CanEditOperationalData);

        var result = Price.Create(
            ctx,
            request.ProductVariantId,
            request.VendorId,
            basePriceResult.Value,
            createdBy: request.VendorId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        context.Prices.Add(result.Value);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Fail(PriceErrors.AlreadyExists());
        }

        return Ok();
    }
}
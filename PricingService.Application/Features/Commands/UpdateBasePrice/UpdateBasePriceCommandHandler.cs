using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Errors;
using PricingService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.UpdateBasePrice;

public sealed class UpdateBasePriceCommandHandler(
    IAppDbContext context,
    IMoneyConverter moneyConverter,
    TimeProvider timeProvider) : IRequestHandler<UpdateBasePriceCommand, IResult>
{
    public async Task<IResult> Handle(UpdateBasePriceCommand request, CancellationToken cancellationToken)
    {
        var product = await context.ProductSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

        if (product == null)
            return Fail(ProductSnapshotErrors.NotFound());

        if (product.VendorId != request.VendorId)
            return Fail(ProductSnapshotErrors.InvalidVendor());

        if (product.Status == ProductCardStatus.Archived)
            return Fail(ProductSnapshotErrors.CannotModifyWhenArchived());

        var price = await context.Prices.FirstOrDefaultAsync(p => 
            p.ProductId == request.ProductId, cancellationToken: cancellationToken);

        if (price == null)
            return Fail(PricingErrors.NotFound());

        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return currencyResult;

        var basePriceResult = Money.FromMinor(request.NewBasePriceMinor, currencyResult.Value);
        if (basePriceResult.IsFailure)
            return basePriceResult;

        var convertedBasePriceResult = await moneyConverter.ConvertAsync(
            basePriceResult.Value,
            Currency.USD,
            cancellationToken);
        if (convertedBasePriceResult.IsFailure) 
            return convertedBasePriceResult;

        var result = price.UpdateBasePrice(convertedBasePriceResult.Value, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}

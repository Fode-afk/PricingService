using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Errors;
using PricingService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.CreatePrice;

public sealed class CreatePriceCommandHandler(
    IAppDbContext context,
    IMoneyConverter moneyConverter,
    TimeProvider timeProvider) : IRequestHandler<CreatePriceCommand, IResult>
{
    public async Task<IResult> Handle(CreatePriceCommand request, CancellationToken cancellationToken)
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

        var exists = await context.Prices.AnyAsync(p =>
            p.ProductId == request.ProductId,
            cancellationToken);
        if (exists)
            return Fail(PricingErrors.AlreadyExists());

        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return currencyResult;  

        var basePriceResult = Money.FromMinor(request.BasePriceMinor, currencyResult.Value);  
        if (basePriceResult.IsFailure)
            return basePriceResult;

        var basePriceInUsdResult = await moneyConverter.ConvertAsync(
            basePriceResult.Value,
            Currency.USD,
            cancellationToken);
        if (basePriceInUsdResult.IsFailure)
            return basePriceInUsdResult;

        var result = Price.Create(
            request.ProductId,
            request.VendorId,
            basePriceInUsdResult.Value,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        context.Prices.Add(result.Value);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}

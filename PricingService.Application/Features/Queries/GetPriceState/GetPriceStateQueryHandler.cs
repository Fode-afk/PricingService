using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Mapping;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetPriceState;

public sealed class GetPriceStateQueryHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<GetPriceStateQuery, IResult<PriceStateDto>>
{
    public async Task<IResult<PriceStateDto>> Handle(GetPriceStateQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<PriceStateDto>(currencyResult.Error);

        var targetCurrency = currencyResult.Value;

        var now = timeProvider.GetUtcNow();

        var price = await context.Prices
            .AsNoTracking()
            .Include(p => p.Entries.Where(e => e.EffectiveTo == null || e.EffectiveTo > now))
            .FirstOrDefaultAsync(p =>
                p.VendorId == request.VendorId &&
                p.ProductVariantId == request.ProductVariantId &&
                p.Currency == targetCurrency, cancellationToken);

        if (price is null)
            return Fail<PriceStateDto>(PriceErrors.NotFound());

        var current = price.CurrentEntry(now);
        var next = price.NextScheduledEntry(now);

        return Ok(
            new PriceStateDto(
                current.ToDto(),
                next?.ToDto()));
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Application.Mapping;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetPriceState;

public sealed class GetPriceStateQueryHandler(
    IAppDbContext context,
    IExchangeRateService exchangeRateService,
    ILogger<GetPriceStateQueryHandler> logger,
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
                p.ProductVariantId == request.ProductVariantId &&
                p.VendorId == request.VendorId, cancellationToken);

        if (price is null)
            return Fail<PriceStateDto>(PriceErrors.NotFound());

        var current = price.CurrentEntry(now);
        var next = price.NextScheduledEntry(now);

        var source = price.Currency;
        var rate = 1m;
        var displayCurrency = source;

        if (targetCurrency != source)
        {
            var fetched = await exchangeRateService.GetExchangeRateAsync(
                source, 
                targetCurrency, cancellationToken);

            if (fetched is null)
            {
                logger.LogWarning(
                    "Exchange rate {Source}→{Target} unavailable, returning price state in {Source} for vendor {VendorId}",
                    source, 
                    targetCurrency,
                    source, 
                    request.VendorId);
            }
            else
            {
                rate = fetched.Value;
                displayCurrency = targetCurrency;
            }
        }

        return Ok(
            new PriceStateDto(
                current.ToDto(rate, displayCurrency),
                next?.ToDto(rate, displayCurrency)));
    }
}

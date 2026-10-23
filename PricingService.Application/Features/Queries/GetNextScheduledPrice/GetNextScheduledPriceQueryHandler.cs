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

namespace PricingService.Application.Features.Queries.GetNextScheduledPrice;

public sealed class GetNextScheduledPriceQueryHandler(
    IAppDbContext context,
    IExchangeRateService exchangeRateService,
    ILogger<GetNextScheduledPriceQueryHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<GetNextScheduledPriceQuery, IResult<PriceEntryDto>>
{
    public async Task<IResult<PriceEntryDto>> Handle(GetNextScheduledPriceQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<PriceEntryDto>(currencyResult.Error);

        var targetCurrency = currencyResult.Value;

        var scheduled = await context.Prices
            .AsNoTracking()
            .Where(p => p.ProductVariantId == request.ProductVariantId && p.VendorId == request.VendorId)
            .SelectMany(p => p.Entries)
            .Where(e => e.EffectiveFrom > timeProvider.GetUtcNow())
            .OrderBy(e => e.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (scheduled is null)
            return Fail<PriceEntryDto>(PriceErrors.NoScheduledPrice());

        var source = scheduled.Amount.Currency;
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
                    "Exchange rate {Source}→{Target} unavailable, returning next scheduled price in {Source} for vendor {VendorId}",
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

        return Ok(scheduled.ToDto(rate, displayCurrency));
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Application.Mapping;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetPriceHistory;

public sealed class GetPriceHistoryQueryHandler(
    IAppDbContext context,
    IExchangeRateService exchangeRateService,
    ILogger<GetPriceHistoryQueryHandler> logger) : IRequestHandler<GetPriceHistoryQuery, IResult<PagedResult<PriceHistoryEntryDto>>>
{
    public async Task<IResult<PagedResult<PriceHistoryEntryDto>>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<PagedResult<PriceHistoryEntryDto>>(currencyResult.Error);

        var targetCurrency = currencyResult.Value;

        var query = context.Prices
            .AsNoTracking()
            .Where(p => p.ProductVariantId == request.ProductVariantId && p.VendorId == request.VendorId)
            .SelectMany(p => p.History);

        if (request.From is not null)
            query = query.Where(h => h.CreatedAt >= request.From);

        if (request.To is not null)
            query = query.Where(h => h.CreatedAt <= request.To);

        var total = await query.CountAsync(cancellationToken);

        var entries = await query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        if (entries.Count == 0 && total == 0)
            return Ok(
                new PagedResult<PriceHistoryEntryDto>(
                    [],
                    total,
                    request.Page,
                    request.PageSize));

        var source = entries.Select(e => e.Amount.Currency).First();
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
                    "Exchange rate {Source}→{Target} unavailable, returning price history in {Source} for vendor {VendorId}",
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
            new PagedResult<PriceHistoryEntryDto>(
                entries.ToDtos(rate, displayCurrency),
                total, 
                request.Page, 
                request.PageSize));
    }
}
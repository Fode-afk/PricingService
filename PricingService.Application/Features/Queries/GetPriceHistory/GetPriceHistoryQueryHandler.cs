using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetPriceHistory;

public sealed class GetPriceHistoryQueryHandler(
    IAppDbContext context,
    IMoneyConverter moneyConverter) : IRequestHandler<GetPriceHistoryQuery, IResult<PagedResult<PriceHistoryEntryDto>>>
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
            return Fail<PagedResult<PriceHistoryEntryDto>>(PriceErrors.NotFound());

        var dtos = new List<PriceHistoryEntryDto>(entries.Count);

        foreach (var e in entries)
        {
            var amountResult = await ConvertUsdToTargetMinorAsync(e.Amount, targetCurrency, cancellationToken);

            if (amountResult.IsFailure)
                return Fail<PagedResult<PriceHistoryEntryDto>>(amountResult.Error);

            dtos.Add(new PriceHistoryEntryDto(
                amountResult.Value,
                e.Reason.ToString(),
                e.ReasonNote,
                e.ChangedBy,
                e.Source,
                e.CreatedAt));
        }

        return Ok(new PagedResult<PriceHistoryEntryDto>(dtos, total, request.Page, request.PageSize));
    }

    private async Task<IResult<long>> ConvertUsdToTargetMinorAsync(
        Money usdAmount,
        Currency targetCurrency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (targetCurrency != Currency.USD)
        {
            var converted = await moneyConverter.ConvertAsync(
                usdAmount,
                targetCurrency,
                cancellationToken);

            if (converted.IsFailure)
                return Fail<long>(converted.Error);

            usdAmount = converted.Value;
        }

        var minorResult = Money.ToMinor(usdAmount.Amount, targetCurrency);

        return minorResult.IsFailure
            ? Fail<long>(minorResult.Error)
            : Ok(minorResult.Value);
    }
}

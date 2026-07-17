using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetScheduledPrice;

public sealed class GetScheduledPriceQueryHandler(
    IAppDbContext context,
    IMoneyConverter moneyConverter,
    TimeProvider timeProvider) : IRequestHandler<GetScheduledPriceQuery, IResult<ScheduledPriceDto>>
{
    public async Task<IResult<ScheduledPriceDto>> Handle(
        GetScheduledPriceQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<ScheduledPriceDto>(currencyResult.Error);

        var targetCurrency = currencyResult.Value;

        var now = timeProvider.GetUtcNow();

        var scheduled = await context.Prices
            .AsNoTracking()
            .Where(p => p.ProductVariantId == request.ProductVariantId && p.VendorId == request.VendorId)
            .SelectMany(p => p.Entries)
            .Where(e => e.EffectiveFrom > now)
            .OrderBy(e => e.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (scheduled is null)
            return Fail<ScheduledPriceDto>(PriceErrors.NoScheduledPrice());

        var amountResult = await ConvertUsdToTargetMinorAsync(scheduled.Amount, targetCurrency, cancellationToken);

        if (amountResult.IsFailure)
            return Fail<ScheduledPriceDto>(amountResult.Error);

        return Ok(new ScheduledPriceDto(amountResult.Value, scheduled.EffectiveFrom, scheduled.EffectiveTo));
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
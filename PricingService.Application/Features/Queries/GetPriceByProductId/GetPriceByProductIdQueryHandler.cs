using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Dtos.CurrencyService;
using migApp.Shared.Results;
using PricingService.Application.Caching;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Errors;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetPriceByProductId;

public sealed class GetPriceByProductIdQueryHandler(
    IFusionCache cache, 
    IAppDbContext context,
    IMoneyConverter moneyConverter) : IRequestHandler<GetPriceByProductIdQuery, IResult<PriceDto>>
{
    public async Task<IResult<PriceDto>> Handle(GetPriceByProductIdQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<PriceDto>(currencyResult.Error);

        var currency = currencyResult.Value;

        var priceDto = await cache.GetOrSetAsync<PriceDto?>(
            CacheKeys.PriceByProductId(request.ProductId, currency.Code),
            async (entry, ct) => await GetPriceDto(request.ProductId, currency, ct),
            tags: [CacheTags.PriceByProductId(request.ProductId)],
            token: cancellationToken);

        if (priceDto == null)
            return Fail<PriceDto>(PricingErrors.NotFound());

        return Ok(priceDto);
    }

    private async Task<PriceDto?> GetPriceDto(
        Guid productId,
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        var price = await context.Prices
            .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);

        if (price == null)
            return null;

        var convertedBasePriceResult = await moneyConverter.ConvertAsync(price.BasePrice, currency, cancellationToken);
        if (convertedBasePriceResult.IsFailure)
            return null;

        var convertedBasePrice = convertedBasePriceResult.Value;

        var convertedBasePriceMinorResult = Money.ToMinor(
            convertedBasePrice.Amount,
            convertedBasePrice.Currency);
        if (convertedBasePriceMinorResult.IsFailure)
            return null;

        var discount = price.Discount;

        long? convertedFixedPriceMinor = null;
        long? convertedAmountOffMinor = null;

        if (discount != null)
        {
            if (discount.FixedPrice != null)
            {
                var convertedFixedPriceResult = await moneyConverter.ConvertAsync(discount.FixedPrice, currency, cancellationToken);
                if (convertedFixedPriceResult.IsFailure)
                    return null;

                var convertedFixedPrice = convertedFixedPriceResult.Value;

                var convertedFixedPriceMinorResult = Money.ToMinor(
                    convertedFixedPrice.Amount,
                    convertedFixedPrice.Currency);
                if (convertedFixedPriceMinorResult.IsFailure)
                    return null;

                convertedFixedPriceMinor = convertedFixedPriceMinorResult.Value;
            }

            if (discount.AmountOff != null)
            {
                var convertedAmountOffResult = await moneyConverter.ConvertAsync(discount.AmountOff, currency, cancellationToken);
                if (convertedAmountOffResult.IsFailure)
                    return null;

                var convertedAmountOff = convertedAmountOffResult.Value;

                var convertedAmountOffMinorResult = Money.ToMinor(
                    convertedAmountOff.Amount,
                    convertedAmountOff.Currency);
                if (convertedAmountOffMinorResult.IsFailure)
                    return null;

                convertedAmountOffMinor = convertedAmountOffMinorResult.Value;
            }
        }

        DiscountDto? discountDto = null;

        if (discount != null)
        {
            discountDto = new DiscountDto(
                discount.DiscountId,
                discount.Type,
                discount.Percentage,
                convertedFixedPriceMinor,
                convertedAmountOffMinor,
                discount.Start,
                discount.End);
        }

        return new PriceDto(
            price.ProductId,
            convertedBasePriceMinorResult.Value,
            currency.Code,
            discountDto);
    }
}
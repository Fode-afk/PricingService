using MediatR;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Enums.Discounts;
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
    IPriceReadRepository priceReadRepository,
    IFusionCache cache, 
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
        var price = await priceReadRepository.GetByProductId(productId, cancellationToken);

        if (price == null)
            return null;

        var convertedBasePriceMinorResult = await ConvertUsdToTargetMinorAsync(
            price.BasePrice,
            currency,
            cancellationToken);
        if (convertedBasePriceMinorResult.IsFailure)
            return null;

        var convertedCurrentPriceMinorResult = await ConvertUsdToTargetMinorAsync(
            price.CurrentPrice,
            currency,
            cancellationToken);
        if (convertedCurrentPriceMinorResult.IsFailure)
            return null;

        long? convertedFixedPriceMinor = null;
        long? convertedAmountOffMinor = null;

        DiscountDto? discountDto = null;

        if (price.DiscountId != null)
        {
            if (price.FixedPrice != null)
            {
                var convertedFixedPriceMinorResult = await ConvertUsdToTargetMinorAsync(
                    price.FixedPrice.Value,
                    currency,
                    cancellationToken);                 
                if (convertedFixedPriceMinorResult.IsFailure)
                    return null;

                convertedFixedPriceMinor = convertedFixedPriceMinorResult.Value;
            }

            if (price.AmountOff != null)
            {
                var convertedAmountOffMinorResult = await ConvertUsdToTargetMinorAsync(
                    price.AmountOff.Value,
                    currency,
                    cancellationToken);          
                if (convertedAmountOffMinorResult.IsFailure)
                    return null;

                convertedAmountOffMinor = convertedAmountOffMinorResult.Value;
            }

            discountDto = new DiscountDto(
                price.DiscountId.Value,
                (DiscountType)price.DiscountType!,
                price.Percentage,
                convertedFixedPriceMinor,
                convertedAmountOffMinor,
                price.CampaignName,
                price.Priority!.Value,
                price.IsStackable!.Value,
                price.DiscountStart!.Value,
                price.DiscountEnd!.Value);
        }

        return new PriceDto(
            price.ProductId,
            convertedBasePriceMinorResult.Value,
            convertedCurrentPriceMinorResult.Value,
            currency.Code,
            discountDto);
    }

    private async Task<IResult<long>> ConvertUsdToTargetMinorAsync(
        decimal usdAmount,
        Currency targetCurrency,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var usdMoneyResult = Money.Create(usdAmount, Currency.USD);
        if (usdMoneyResult.IsFailure)
            return Fail<long>(usdMoneyResult.Error);

        var money = usdMoneyResult.Value;

        if (targetCurrency != Currency.USD)
        {
            var converted = await moneyConverter.ConvertAsync(
                money,
                targetCurrency,
                cancellationToken);

            if (converted.IsFailure)
                return Fail<long>(converted.Error);

            money = converted.Value;
        }

        var minorResult = Money.ToMinor(money.Amount, targetCurrency);

        return minorResult.IsFailure
            ? Fail<long>(minorResult.Error)
            : Ok(minorResult.Value);
    }
}
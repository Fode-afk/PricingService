using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Enums.Discounts;
using migApp.Shared.Messaging.IntegrationEvents.Discounts;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.ApplyDiscount;

public sealed class ApplyDiscountCommandHandler(
    IAppDbContext context,
    IPublishEndpoint publish,
    TimeProvider timeProvider) : IRequestHandler<ApplyDiscountCommand, IResult>
{
    public async Task<IResult> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        var price = await context.Prices.FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

        if (price == null)
        {
            await publish.Publish(new DiscountProcessFailed(
                request.CorrelationId,
                "Price not found"), cancellationToken);
            return Ok();
        }

        var discountAmountInUsdResult = Money.FromMinor(request.DiscountAmountInUsdMinor, Currency.USD);

        if (discountAmountInUsdResult.IsFailure)
        {
            await publish.Publish(new DiscountProcessFailed(
                request.CorrelationId,
                discountAmountInUsdResult.Error.Code), cancellationToken);
            return Ok();
        }

        var discountAmountInUsd = discountAmountInUsdResult.Value;

        DiscountSnapshot? discountSnapshot = null;
        //switch (request.DiscountType)
        //{
        //    case DiscountType.AmountOff:
        //        discountSnapshot = DiscountSnapshot.CreateAmountOff(
        //            request.DiscountId, 
        //            discountAmountInUsd, 
        //            request.Start,
        //            request.End);
        //        break;
        //    case DiscountType.FixedPrice:
        //        discountSnapshot = DiscountSnapshot.CreateFixed(
        //            request.DiscountId,
        //            discountAmountInUsd,
        //            request.Start,
        //            request.End);
        //        break;
        //    default:
        //        break;
        //}

        if (discountSnapshot == null)
        {
            await publish.Publish(new DiscountProcessFailed(
                request.CorrelationId,
                "Invalid snapshot"), cancellationToken);
            return Ok();
        }    

        var result = price.SetDiscount(discountSnapshot, timeProvider.GetUtcNow());

        if (result.IsFailure)
        {
            await publish.Publish(new DiscountProcessFailed(
                request.CorrelationId,
                result.Error.Code), cancellationToken);
            return Ok();
        }

        return Ok();
    }
}

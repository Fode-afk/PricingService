using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Messaging.IntegrationEvents.Discounts;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetBasePriceByProductId;

public sealed class GetBasePriceByProductIdQueryHandler(
    IAppDbContext context,
    IPublishEndpoint publish) : IRequestHandler<GetBasePriceByProductIdQuery, IResult>
{
    public async Task<IResult> Handle(GetBasePriceByProductIdQuery request, CancellationToken cancellationToken)
    {
        var basePriceInUsd = await context.Prices
            .Where(p => p.ProductId == request.ProductId)
            .Select(p => (decimal?)p.BasePrice.Amount)
            .FirstOrDefaultAsync(cancellationToken);

        if (basePriceInUsd == null)
        {
            await PublishFailure(
                request.CorrelationId,
                "ProductBasePrice not found",
                cancellationToken);
            return Ok();
        }

        var basePriceInUsdMinorResult = Money.ToMinor(basePriceInUsd.Value, Currency.USD);

        if (basePriceInUsdMinorResult.IsFailure)
        {
            await PublishFailure(
                request.CorrelationId,
                basePriceInUsdMinorResult.Error.Code,
                cancellationToken);
            return Ok();
        }

        await publish.Publish(new BasePriceRetrieved(request.CorrelationId, basePriceInUsdMinorResult.Value), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    private async Task PublishFailure(Guid correlationId, string reason, CancellationToken cancellationToken = default)
    {
        await publish.Publish(new DiscountProcessFailed(correlationId, reason), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

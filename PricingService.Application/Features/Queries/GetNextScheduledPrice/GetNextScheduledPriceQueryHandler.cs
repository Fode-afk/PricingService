using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Mapping;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetNextScheduledPrice;

public sealed class GetNextScheduledPriceQueryHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<GetNextScheduledPriceQuery, IResult<PriceEntryDto>>
{
    public async Task<IResult<PriceEntryDto>> Handle(GetNextScheduledPriceQuery request, CancellationToken cancellationToken)
    {
        var marketResult = Market.Create(request.Market);
        if (marketResult.IsFailure)
            return Fail<PriceEntryDto>(marketResult.Error);

        var scheduled = await context.Prices
            .AsNoTracking()
            .Where(p => 
                p.VendorId == request.VendorId &&
                p.ProductVariantId == request.ProductVariantId &&
                p.Market == marketResult.Value)
            .SelectMany(p => p.Entries)
            .Where(e => e.EffectiveFrom > timeProvider.GetUtcNow())
            .OrderBy(e => e.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (scheduled is null)
            return Fail<PriceEntryDto>(PriceErrors.NoScheduledPrice());

        return Ok(scheduled.ToDto());
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Mapping;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetVariantPrices;

public sealed class GetVariantPricesQueryHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<GetVariantPricesQuery, IResult<IReadOnlyList<VariantPriceDto>>>
{
    public async Task<IResult<IReadOnlyList<VariantPriceDto>>> Handle(GetVariantPricesQuery request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();

        var prices = await context.Prices
            .AsNoTracking()
            .Include(p => p.Entries.Where(e => e.EffectiveTo == null || e.EffectiveTo > now))
            .Where(p => 
                p.VendorId == p.VendorId &&
                p.ProductVariantId == p.ProductVariantId)
            .ToListAsync(cancellationToken);

        return Ok<IReadOnlyList<VariantPriceDto>>([..prices
            .Select(p => p.ToDto(now))
            .OrderByDescending(x => x.IsBase)
            .ThenBy(x => x.Currency, StringComparer.Ordinal)]);
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using PricingService.Application.Dtos;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Mapping;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Queries.GetPriceHistory;

public sealed class GetPriceHistoryQueryHandler(IAppDbContext context) : IRequestHandler<GetPriceHistoryQuery, IResult<PagedResult<PriceHistoryEntryDto>>>
{
    public async Task<IResult<PagedResult<PriceHistoryEntryDto>>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        var marketResult = Market.Create(request.Market);
        if (marketResult.IsFailure)
            return Fail<PagedResult<PriceHistoryEntryDto>>(marketResult.Error);

        var query = context.Prices
            .AsNoTracking()
            .Where(p => 
                p.VendorId == request.VendorId &&
                p.ProductVariantId == request.ProductVariantId &&
                p.Market == marketResult.Value)
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

        return Ok(
            new PagedResult<PriceHistoryEntryDto>(
                entries.ToDtos(),
                total, 
                request.Page, 
                request.PageSize));
    }
}
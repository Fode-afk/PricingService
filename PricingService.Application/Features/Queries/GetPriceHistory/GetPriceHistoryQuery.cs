using MediatR;
using migApp.Shared.Results;
using PricingService.Application.Dtos;

namespace PricingService.Application.Features.Queries.GetPriceHistory;

public sealed record GetPriceHistoryQuery(
    Guid ProductVariantId,
    Guid VendorId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int Page,
    int PageSize,
    string Currency) : IRequest<IResult<PagedResult<PriceHistoryEntryDto>>>;
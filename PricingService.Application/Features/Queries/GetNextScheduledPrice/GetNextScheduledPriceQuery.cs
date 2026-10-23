using MediatR;
using migApp.Shared.Results;
using PricingService.Application.Dtos;

namespace PricingService.Application.Features.Queries.GetNextScheduledPrice;

public sealed record GetNextScheduledPriceQuery(
    Guid ProductVariantId,
    Guid VendorId,
    string Currency) : IRequest<IResult<PriceEntryDto>>;
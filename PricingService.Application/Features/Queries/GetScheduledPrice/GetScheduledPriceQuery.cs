using MediatR;
using migApp.Shared.Results;
using PricingService.Application.Dtos;

namespace PricingService.Application.Features.Queries.GetScheduledPrice;

public sealed record GetScheduledPriceQuery(
    Guid ProductVariantId,
    Guid VendorId,
    string Currency) : IRequest<IResult<ScheduledPriceDto>>;
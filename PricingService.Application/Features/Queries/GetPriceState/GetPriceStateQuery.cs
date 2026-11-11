using MediatR;
using migApp.Shared.Results;
using PricingService.Application.Dtos;

namespace PricingService.Application.Features.Queries.GetPriceState;

public sealed record GetPriceStateQuery(
    Guid ProductVariantId,
    Guid VendorId,
    string Market) : IRequest<IResult<PriceStateDto>>;
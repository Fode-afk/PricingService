using MediatR;
using migApp.Shared.Results;
using PricingService.Application.Dtos;

namespace PricingService.Application.Features.Queries.GetPriceByProductId;

public sealed record GetPriceByProductIdQuery(Guid ProductId, string Currency) : IRequest<IResult<PriceDto>>;
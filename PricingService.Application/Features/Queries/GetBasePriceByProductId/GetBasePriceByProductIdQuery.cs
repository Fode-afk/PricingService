using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Queries.GetBasePriceByProductId;

public sealed record GetBasePriceByProductIdQuery(Guid CorrelationId, Guid ProductId) : IRequest<IResult>;
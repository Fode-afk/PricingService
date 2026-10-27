using MediatR;
using migApp.Shared.Results;
using PricingService.Application.Dtos;

namespace PricingService.Application.Features.Queries.GetVariantPrices;

public sealed record GetVariantPricesQuery(
    Guid ProductVariantId,
    Guid VendorId) : IRequest<IResult<IReadOnlyList<VariantPriceDto>>>;
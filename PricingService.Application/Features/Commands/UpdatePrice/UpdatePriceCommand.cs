using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.UpdatePrice;

public sealed record UpdatePriceCommand(
    Guid VendorId,
    Guid ProductVariantId,
    long NewPriceMinor,
    string Currency,
    DateTimeOffset? EffectiveFrom = null,
    DateTimeOffset? EffectiveTo = null) : IRequest<IResult>;
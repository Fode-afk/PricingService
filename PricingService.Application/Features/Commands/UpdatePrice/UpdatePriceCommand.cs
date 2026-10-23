using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.UpdatePrice;

public sealed record UpdatePriceCommand(
    Guid ProductVariantId,
    Guid VendorId,
    long NewPriceMinor,
    string Currency,
    DateTimeOffset? EffectiveFrom,
    DateTimeOffset? EffectiveTo) : IRequest<IResult>;
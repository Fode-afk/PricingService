using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.UpdatePrice;

public sealed record UpdatePriceCommand(
    Guid ProductVariantId,
    Guid VendorId,
    long NewPriceMinor,
    string Market,
    DateTimeOffset? EffectiveFrom,
    DateTimeOffset? EffectiveTo) : IRequest<IResult>;
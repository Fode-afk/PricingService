using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.CreatePrice;

public sealed record CreatePriceCommand(
    Guid ProductVariantId,
    Guid VendorId, 
    long BasePriceMinor,
    string Currency,
    DateTimeOffset? EffectiveFrom) : IRequest<IResult>;
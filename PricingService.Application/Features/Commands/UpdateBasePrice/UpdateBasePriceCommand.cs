using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.UpdateBasePrice;

public sealed record UpdateBasePriceCommand(
    Guid ProductId,
    Guid VendorId,
    long NewBasePriceMinor,
    string Currency) : IRequest<IResult>;
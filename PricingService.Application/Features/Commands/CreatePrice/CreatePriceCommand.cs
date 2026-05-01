using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.CreatePrice;

public sealed record CreatePriceCommand(
    Guid ProductId,
    Guid VendorId, 
    long BasePriceMinor,
    string Currency) : IRequest<IResult>;
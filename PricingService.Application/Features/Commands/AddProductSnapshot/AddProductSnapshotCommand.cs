using MediatR;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.AddProductSnapshot;

public sealed record AddProductSnapshotCommand(
    Guid ProductId,
    Guid VendorId, 
    Guid ProductCardId) : IRequest<IResult>;
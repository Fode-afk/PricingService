using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using PricingService.Api.Grpc.Mappers;
using PricingService.Api.Grpc.V1.Protos;
using PricingService.Application.Features.Queries.GetPriceByProductId;

namespace PricingService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.PricingService.PricingServiceBase
{
    public override async Task<Empty> CreatePrice(CreatePriceRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            PriceGrpcMapper.ToCreatePriceCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<GetPriceByProductIdResponse> GetPriceByProductId(GetPriceByProductIdRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetPriceByProductIdQuery(
            Guid.Parse(request.ProductId),
            request.Currency), 
            context.CancellationToken);
        return new GetPriceByProductIdResponse
        {
            Price = PriceGrpcMapper.ToDto(result.ThrowIfFailure())
        };
    }
}

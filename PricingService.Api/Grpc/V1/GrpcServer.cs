using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using PricingService.Api.Grpc.Mapping;
using PricingService.Api.Grpc.V1.Protos;

namespace PricingService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.PricingService.PricingServiceBase
{
    public override async Task<Empty> CreatePrice(CreatePriceRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToCreatePriceCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdatePrice(UpdatePriceRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToUpdatePriceCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<GetPriceHistoryResponse> GetPriceHistory(GetPriceHistoryRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToGetPriceHistoryQuery(),
            context.CancellationToken);
        return result.ThrowIfFailure().ToGetPriceHistoryResponse();
    }

    public override async Task<GetScheduledPriceResponse> GetScheduledPrice(GetScheduledPriceRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToGetScheduledPriceQuery(),
            context.CancellationToken);
        return new GetScheduledPriceResponse
        { 
            ScheduledPrice = result.ThrowIfFailure().ToGrpc()
        };
    }
}

using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using PricingService.Api.Grpc.Mapping;
using PricingService.Api.Grpc.V1.Protos;
using PricingService.Application.Features.Queries.GetPriceState;

namespace PricingService.Api.Grpc.V1;

internal sealed class GrpcServer(ISender sender) : Protos.PricingService.PricingServiceBase
{
    public override async Task<PriceStateResponse> CreatePrice(CreatePriceRequest request, ServerCallContext context)
    {
        var result = await sender.Send(
            request.ToCreatePriceCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();

        return await GetPriceState(
            request.ProductVariantId,
            request.VendorId,
            request.MarketCode,
            context.CancellationToken);
    }

    public override async Task<PriceStateResponse> UpdatePrice(UpdatePriceRequest request, ServerCallContext context)
    {
        var result = await sender.Send(
            request.ToUpdatePriceCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();

        return await GetPriceState(
            request.ProductVariantId,
            request.VendorId,
            request.MarketCode,
            context.CancellationToken);
    }

    public override async Task<GetPriceHistoryResponse> GetPriceHistory(GetPriceHistoryRequest request, ServerCallContext context) =>
        (await sender.Send(
            request.ToGetPriceHistoryQuery(),
            context.CancellationToken)).ThrowIfFailure().ToGetPriceHistoryResponse();

    public override async Task<PriceEntryDto> GetNextScheduledPrice(GetNextScheduledPriceRequest request, ServerCallContext context) => 
        (await sender.Send(
            request.ToGetNextScheduledPriceQuery(),
            context.CancellationToken)).ThrowIfFailure().ToGrpc();

    public override async Task<GetVariantPricesResponse> GetVariantPrices(GetVariantPricesRequest request, ServerCallContext context)
    {
        var result = await sender.Send(
             request.ToGetVariantPricesQuery(),
             context.CancellationToken);
        return new GetVariantPricesResponse
        {
            VariantPrices = { result.ThrowIfFailure().Select(p => p.ToGrpc()) }
        };
    }

    private async Task<PriceStateResponse> GetPriceState(
        string productVariantId,
        string vendorId,
        string marketCode,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await sender.Send(
            new GetPriceStateQuery(
                Guid.Parse(productVariantId),
                Guid.Parse(vendorId),
                marketCode), cancellationToken);

        var state = queryResult.ThrowIfFailure();

        return new PriceStateResponse
        {
            Current = state.Current.ToGrpc(),
            NextScheduled = state.NextScheduled?.ToGrpc()
        };
    }
}

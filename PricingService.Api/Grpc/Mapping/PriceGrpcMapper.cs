using Google.Protobuf.WellKnownTypes;
using migApp.Shared.Results;
using PricingService.Api.Grpc.V1.Protos;
using PricingService.Application.Features.Commands.CreatePrice;
using PricingService.Application.Features.Commands.UpdatePrice;
using PricingService.Application.Features.Queries.GetNextScheduledPrice;
using PricingService.Application.Features.Queries.GetPriceHistory;
using PricingService.Application.Features.Queries.GetVariantPrices;
using Dtos = PricingService.Application.Dtos;

namespace PricingService.Api.Grpc.Mapping;

internal static class PriceGrpcMapper
{
    public static CreatePriceCommand ToCreatePriceCommand(this CreatePriceRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            request.BasePriceMinor,
            request.MarketCode);

    public static UpdatePriceCommand ToUpdatePriceCommand(this UpdatePriceRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            request.NewPriceMinor,
            request.MarketCode,
            request.HasEffectiveFrom ? DateTimeOffset.Parse(request.EffectiveFrom) : null,
            request.HasEffectiveTo ? DateTimeOffset.Parse(request.EffectiveTo) : null);

    public static GetPriceHistoryQuery ToGetPriceHistoryQuery(this GetPriceHistoryRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            request.HasFrom ? DateTimeOffset.Parse(request.From) : null,
            request.HasTo ? DateTimeOffset.Parse(request.To) : null,
            request.Page,
            request.PageSize,
            request.MarketCode);

    public static GetNextScheduledPriceQuery ToGetNextScheduledPriceQuery(this GetNextScheduledPriceRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            request.MarketCode);

    public static GetPriceHistoryResponse ToGetPriceHistoryResponse(this PagedResult<Dtos.PriceHistoryEntryDto> pagedResult) =>
        new()
        {
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            TotalPages = pagedResult.TotalPages,
            PageSize = pagedResult.PageSize,
            Entries = { pagedResult.Items.Select(e => e.ToGrpc()) }
        };

    public static GetVariantPricesQuery ToGetVariantPricesQuery(this GetVariantPricesRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId));

    public static PriceHistoryEntryDto ToGrpc(this Dtos.PriceHistoryEntryDto dto)
    {
        var result = new PriceHistoryEntryDto
        {
            PriceMinor = dto.AmountMinor,
            Currency = dto.Currency,
            Reason = dto.Reason,
            ChangedBy = dto.ChangedBy.ToString(),
            Source = dto.Source
        };

        if (dto.ReasonNote != null)
            result.ReasonNote = dto.ReasonNote;

        return result;
    }

    public static PriceEntryDto ToGrpc(this Dtos.PriceEntryDto dto)
    {
        var result = new PriceEntryDto
        {
            PriceMinor = dto.PriceMinor,
            CurrencyCode = dto.Currency,
            EffectiveFrom = Timestamp.FromDateTimeOffset(dto.EffectiveFrom),
        };

        if (dto.EffectiveTo.HasValue)
            result.EffectiveTo = Timestamp.FromDateTimeOffset(dto.EffectiveTo.Value);

        return result;
    }

    public static VariantPriceDto ToGrpc(this Dtos.VariantPriceDto dto) =>
        new()
        { 
            MarketCode = dto.Market,
            CurrentPriceMinor = dto.CurrentPriceMinor,
            CurrencyCode = dto.Currency
        };
}
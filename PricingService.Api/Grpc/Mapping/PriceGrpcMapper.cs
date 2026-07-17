using Google.Protobuf.WellKnownTypes;
using migApp.Shared.Results;
using PricingService.Api.Grpc.V1.Protos;
using PricingService.Application.Features.Commands.CreatePrice;
using PricingService.Application.Features.Commands.UpdatePrice;
using PricingService.Application.Features.Queries.GetPriceHistory;
using PricingService.Application.Features.Queries.GetScheduledPrice;
using Dtos = PricingService.Application.Dtos;

namespace PricingService.Api.Grpc.Mapping;

internal static class PriceGrpcMapper
{
    public static CreatePriceCommand ToCreatePriceCommand(this CreatePriceRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            BasePriceMinor: request.BasePriceMinor,
            Currency: request.CurrencyCode,
            EffectiveFrom: request.EffectiveFrom.ToDateTimeOffset());

    public static UpdatePriceCommand ToUpdatePriceCommand(this UpdatePriceRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            NewPriceMinor: request.NewPriceMinor,
            Currency: request.CurrencyCode,
            EffectiveFrom: request.EffectiveFrom.ToDateTimeOffset(),
            EffectiveTo: request.EffectiveTo.ToDateTimeOffset());

    public static DateTimeOffset? ToDateTimeOffset(this Timestamp timestamp) =>
        timestamp == null || timestamp == Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime())
            ? null
            : timestamp.ToDateTimeOffset();

    public static GetPriceHistoryQuery ToGetPriceHistoryQuery(this GetPriceHistoryRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            From: request.From.ToDateTimeOffset(),
            To: request.To.ToDateTimeOffset(),
            Page: request.Page,
            PageSize: request.PageSize,
            Currency: request.CurrencyCode);

    public static GetScheduledPriceQuery ToGetScheduledPriceQuery(this GetScheduledPriceRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            Currency: request.CurrencyCode);

    public static GetPriceHistoryResponse ToGetPriceHistoryResponse(this PagedResult<Dtos.PriceHistoryEntryDto> pagedResult) =>
        new()
        {
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            TotalPages = pagedResult.TotalPages,
            PageSize = pagedResult.PageSize,
            Entries =
            {
                pagedResult.Items.Select(e => e.ToGrpc())
            }
        };

    public static PriceHistoryEntryDto ToGrpc(this Dtos.PriceHistoryEntryDto dto)
    {
        var result = new PriceHistoryEntryDto
        {
            PriceMinor = dto.AmountMinor,
            Reason = dto.Reason,
            ChangedBy = dto.ChangedBy.ToString(),
            Source = dto.Source
        };

        if (dto.ReasonNote != null)
            result.ReasonNote = dto.ReasonNote;

        return result;
    }

    public static ScheduledPriceDto ToGrpc(this Dtos.ScheduledPriceDto dto)
    {
        var result = new ScheduledPriceDto
        {
            PriceMinor = dto.AmountMinor,
            EffectiveFrom = Timestamp.FromDateTime(dto.EffectiveFrom.UtcDateTime),
        };

        if (dto.EffectiveTo.HasValue)
            result.EffectiveTo = Timestamp.FromDateTime(dto.EffectiveTo.Value.UtcDateTime);

        return result;
    }
}

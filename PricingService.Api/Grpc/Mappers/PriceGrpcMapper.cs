using Google.Protobuf.WellKnownTypes;
using PricingService.Api.Grpc.V1.Protos;
using PricingService.Application.Features.Commands.CreatePrice;

namespace PricingService.Api.Grpc.Mappers;

internal static class PriceGrpcMapper
{
    public static CreatePriceCommand ToCreatePriceCommand(CreatePriceRequest request) =>
        new(
            ProductId: Guid.Parse(request.ProductId),
            VendorId: Guid.Parse(request.VendorId),
            BasePriceMinor: request.BasePriceMinor,
            Currency: request.Currency);

    public static PriceDto ToDto(Application.Dtos.PriceDto price) =>
        new()
        {
            ProductId = price.ProductId.ToString(),
            BasePriceMinor = price.BasePriceMinor,
            CurrentPriceMinor = price.CurrentPriceMinor,
            Currency = price.Currency,
            Discount = price.Discount == null ? null : ToDiscount(price.Discount)
        };

    public static DiscountDto ToDiscount(Application.Dtos.DiscountDto discount) =>
        new()
        {
            DiscountId = discount.DiscountId.ToString(),
            Type = (DiscountType)discount.Type,
            Percentage = (double?)discount.Percentage ?? 0,
            AmountOffMinor = discount.AmountOffMinor ?? 0,
            FixedPriceMinor = discount.FixedPriceMinor ?? 0,
            CampaignName = discount.CampaignName,
            Priority = discount.Priority,
            IsStackable = discount.IsStackable,
            Start = Timestamp.FromDateTimeOffset(discount.Start),
            End = Timestamp.FromDateTimeOffset(discount.End)
        };
}

using MediatR;
using migApp.Shared.Enums.Discounts;
using migApp.Shared.Results;

namespace PricingService.Application.Features.Commands.ApplyDiscount;

public sealed record ApplyDiscountCommand(
    Guid CorrelationId,
    Guid ProductId,
    Guid DiscountId,
    DiscountType DiscountType,
    long DiscountAmountInUsdMinor,
    DateTimeOffset Start,
    DateTimeOffset End) : IRequest<IResult>;
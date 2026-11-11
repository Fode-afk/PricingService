using FluentValidation;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Errors;

namespace PricingService.Application.Features.Queries.GetPriceState;

public sealed class GetPriceStateQueryValidator : AbstractValidator<GetPriceStateQuery>
{
    public GetPriceStateQueryValidator()
    {
        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.Market)
            .NotEmpty()
            .Length(Market.CodeLength)
            .WithErrorCode(MarketErrorCodes.InvalidFormat);
    }
}

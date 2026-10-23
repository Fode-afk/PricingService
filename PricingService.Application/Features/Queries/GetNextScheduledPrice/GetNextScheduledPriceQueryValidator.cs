using FluentValidation;
using migApp.Shared.Domain.Errors;
using PricingService.Domain.Errors;

namespace PricingService.Application.Features.Queries.GetNextScheduledPrice;

public sealed class GetNextScheduledPriceQueryValidator : AbstractValidator<GetNextScheduledPriceQuery>
{
    public GetNextScheduledPriceQueryValidator()
    {
        RuleFor(x => x.ProductVariantId)
          .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithErrorCode(CurrencyErrorCodes.InvalidFormat);
    }
}
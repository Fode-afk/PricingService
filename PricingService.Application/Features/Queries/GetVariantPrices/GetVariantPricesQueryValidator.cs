using FluentValidation;
using PricingService.Domain.Errors;

namespace PricingService.Application.Features.Queries.GetVariantPrices;

public sealed class GetVariantPricesQueryValidator : AbstractValidator<GetVariantPricesQuery>
{
    public GetVariantPricesQueryValidator()
    {
        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);
    }
}

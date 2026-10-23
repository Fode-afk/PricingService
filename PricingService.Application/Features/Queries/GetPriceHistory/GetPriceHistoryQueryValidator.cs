using FluentValidation;
using migApp.Shared.Domain.Errors;
using PricingService.Domain.Errors;

namespace PricingService.Application.Features.Queries.GetPriceHistory;

public sealed class GetPriceHistoryQueryValidator : AbstractValidator<GetPriceHistoryQuery>
{ 
    public GetPriceHistoryQueryValidator()
    {
        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.From)
            .LessThanOrEqualTo(x => x.To)
            .When(x => x.From.HasValue && x.To.HasValue)
            .WithErrorCode(PriceEntryErrorCodes.InvalidTerms);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(PriceEntryErrorCodes.InvalidTerms);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithErrorCode(PriceErrorCodes.InvalidPagination);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithErrorCode(CurrencyErrorCodes.InvalidFormat);
    }
}
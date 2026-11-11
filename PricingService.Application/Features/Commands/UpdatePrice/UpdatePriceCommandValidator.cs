using FluentValidation;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Errors;

namespace PricingService.Application.Features.Commands.UpdatePrice;

public sealed class UpdatePriceCommandValidator : AbstractValidator<UpdatePriceCommand>
{
    public UpdatePriceCommandValidator()
    {
        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.NewPriceMinor)
            .GreaterThan(0).WithErrorCode(MoneyErrorCodes.AmountNegative);

        RuleFor(x => x.Market)
            .NotEmpty()
            .Length(Market.CodeLength)
            .WithErrorCode(MarketErrorCodes.InvalidFormat);

        RuleFor(x => x)
            .Must(x => x.EffectiveFrom is null || x.EffectiveTo is null || x.EffectiveTo > x.EffectiveFrom)
            .WithErrorCode(PriceEntryErrorCodes.InvalidTerms);
    }
}
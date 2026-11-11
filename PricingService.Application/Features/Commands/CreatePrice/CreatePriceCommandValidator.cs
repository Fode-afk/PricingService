using FluentValidation;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Errors;

namespace PricingService.Application.Features.Commands.CreatePrice;

public sealed class CreatePriceCommandValidator : AbstractValidator<CreatePriceCommand>
{
    public CreatePriceCommandValidator()
    {
        RuleFor(x => x.ProductVariantId)
          .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(PriceErrorCodes.InvalidId);

        RuleFor(x => x.BasePriceMinor)
            .GreaterThan(0).WithErrorCode(MoneyErrorCodes.AmountNegative);

        RuleFor(x => x.Market)
            .NotEmpty()
            .Length(Market.CodeLength)
            .WithErrorCode(MarketErrorCodes.InvalidFormat);
    }
}
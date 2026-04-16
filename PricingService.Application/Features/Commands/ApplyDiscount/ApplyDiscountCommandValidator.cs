using FluentValidation;

namespace PricingService.Application.Features.Commands.ApplyDiscount;

public sealed class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    public ApplyDiscountCommandValidator()
    { 
        
    }
}

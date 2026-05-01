using FluentValidation;

namespace PricingService.Application.Features.Commands.UpdateBasePrice;

public sealed class UpdateBasePriceCommandValidator : AbstractValidator<UpdateBasePriceCommand>
{
    public UpdateBasePriceCommandValidator()
    {
        
    }
}

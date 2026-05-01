using FluentValidation;

namespace PricingService.Application.Features.Commands.CreatePrice;

public sealed class CreatePriceCommandValidator : AbstractValidator<CreatePriceCommand>
{
    public CreatePriceCommandValidator()
    {
        
    }
}
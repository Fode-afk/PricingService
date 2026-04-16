using FluentValidation;

namespace PricingService.Application.Features.Queries.GetBasePriceByProductId;

public sealed class GetBasePriceByProductIdQueryValidator : AbstractValidator<GetBasePriceByProductIdQuery>
{
    public GetBasePriceByProductIdQueryValidator()
    {

    }
}

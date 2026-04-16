using FluentValidation;

namespace PricingService.Application.Features.Queries.GetPriceByProductId;

public sealed class GetPriceByProductIdQueryValidator : AbstractValidator<GetPriceByProductIdQuery>
{
    public GetPriceByProductIdQueryValidator()
    {

    }
}

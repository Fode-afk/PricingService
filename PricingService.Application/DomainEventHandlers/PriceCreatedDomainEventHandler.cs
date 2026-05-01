using PricingService.Application.Interfaces.Data;
using PricingService.Domain.DomainEvents;
using PricingService.Domain.Models;
using PricingService.Domain.Primitives;

namespace PricingService.Application.DomainEventHandlers;

public sealed class PriceCreatedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<PriceCreatedDomainEvent>
{
    public Task Handle(PriceCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var price = notification.Price;
        var discount = notification.Price.AppliedDiscount;

        var readModel = new PriceReadModel
        {
            Id = price.Id,
            ProductId = price.ProductId,
            BasePrice = price.BasePrice.Amount,
            CurrentPrice = price.CurrentPrice.Amount,
            DiscountId = discount?.DiscountId,
            DiscountType = (int?)discount?.Type,
            Percentage = discount?.Percentage,
            FixedPrice = discount?.FixedPrice?.Amount,
            AmountOff = discount?.AmountOff?.Amount,
            CampaignName = discount?.CampaignName?.Value,
            Priority = discount?.Priority.Value,
            IsStackable = discount?.IsStackable,
            DiscountStart = discount?.Start,
            DiscountEnd = discount?.End,
            UpdatedAt = price.UpdatedAt,
            CreatedAt = price.CreatedAt
        };

        context.PriceReadModels.Add(readModel);

        return Task.CompletedTask;
    }
}

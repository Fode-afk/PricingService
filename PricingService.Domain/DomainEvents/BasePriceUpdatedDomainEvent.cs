using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record BasePriceUpdatedDomainEvent(
    Guid PriceId,
    Guid ProductId,
    Money NewPrice) : IDomainEvent;

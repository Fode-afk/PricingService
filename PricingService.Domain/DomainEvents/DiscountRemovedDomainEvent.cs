using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record DiscountRemovedDomainEvent(Guid PriceId) : IDomainEvent;
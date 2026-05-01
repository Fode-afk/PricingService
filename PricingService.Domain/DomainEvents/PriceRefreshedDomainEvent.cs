using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceRefreshedDomainEvent(Guid PriceId) : IDomainEvent;
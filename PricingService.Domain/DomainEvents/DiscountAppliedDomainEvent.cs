using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record DiscountAppliedDomainEvent(Guid PriceId, Guid DiscountId) : IDomainEvent;
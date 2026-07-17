using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceArchivedDomainEvent(Guid ProductVariantId) : IDomainEvent;
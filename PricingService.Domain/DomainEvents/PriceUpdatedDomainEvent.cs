using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceUpdatedDomainEvent(
    Guid ProductVariantId,
    bool HasActivePrice, 
    long Version) : IDomainEvent;

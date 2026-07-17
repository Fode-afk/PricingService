using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceCreatedDomainEvent(
    Guid ProductVariantId,
    bool HasActivePrice,
    long Version) : IDomainEvent;
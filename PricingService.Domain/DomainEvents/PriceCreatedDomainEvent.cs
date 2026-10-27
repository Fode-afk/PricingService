using migApp.Shared.Domain.Primitives;
using migApp.Shared.Domain.ValueObjects;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceCreatedDomainEvent(
    Guid PriceId,
    Guid ProductVariantId,
    bool IsBase,
    Money CurrentPrice,
    long Version) : IDomainEvent;
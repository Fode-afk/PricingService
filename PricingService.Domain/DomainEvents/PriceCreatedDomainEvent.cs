using migApp.Shared.Domain.Primitives;
using migApp.Shared.Domain.ValueObjects;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceCreatedDomainEvent(
    Guid ProductVariantId,
    Money CurrentPrice,
    long Version) : IDomainEvent;
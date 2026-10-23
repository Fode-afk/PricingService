using migApp.Shared.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceArchivedDomainEvent(Guid ProductVariantId) : IDomainEvent;
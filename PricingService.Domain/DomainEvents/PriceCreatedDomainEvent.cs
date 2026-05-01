using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Models;
using PricingService.Domain.Primitives;

namespace PricingService.Domain.DomainEvents;

public sealed record PriceCreatedDomainEvent(
    Guid VendorId,
    Price Price) : IDomainEvent;
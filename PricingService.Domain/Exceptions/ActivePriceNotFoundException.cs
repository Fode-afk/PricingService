namespace PricingService.Domain.Exceptions;

public sealed class ActivePriceNotFoundException(Guid priceId)
    : Exception($"Price with ID {priceId} does not have an active price entry. " +
                $"This indicates that the Price aggregate invariant has been violated.");
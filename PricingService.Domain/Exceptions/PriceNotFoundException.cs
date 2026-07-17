namespace PricingService.Domain.Exceptions;

public sealed class PriceNotFoundException(Guid priceId)
    : Exception($"Price with ID {priceId} not found."), IExpectedException;

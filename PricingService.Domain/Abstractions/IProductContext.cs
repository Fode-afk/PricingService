namespace PricingService.Domain.Abstractions;

public interface IProductContext
{
    bool ProductCanEditOperationalData { get; }
}
namespace PricingService.Domain.Abstractions;

public interface IProductContext
{
    bool ProductCanBeModified { get; }
}

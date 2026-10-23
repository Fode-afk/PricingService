namespace PricingService.Domain.Abstractions;

internal interface IProductContext
{
    bool ProductCanEditOperationalData { get; }
}
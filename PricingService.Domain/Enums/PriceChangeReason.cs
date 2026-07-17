namespace PricingService.Domain.Enums;

public enum PriceChangeReason
{
    Initial,
    ManualUpdate,
    BulkImport, 
    PriceRule,
    ScheduledEntry
}
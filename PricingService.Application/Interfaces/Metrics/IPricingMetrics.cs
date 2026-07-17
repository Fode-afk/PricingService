namespace PricingService.Application.Interfaces.Metrics;

public interface IPricingMetrics
{
    void RecordPriceNotFound();
    void RecordSnapshotNotFound(string snapshotType);
    void RecordSnapshotOutdated(string handlerName);
    void RecordHandlerError(string handlerName, string handlerType);
    void RecordHandlerDuration(double ms, string handlerName, string handlerType);
}
using PricingService.Application.Interfaces.Metrics;
using System.Diagnostics.Metrics;

namespace PricingService.Infrastructure.Observability;

public sealed class PricingServiceMetrics : IDisposable, IPricingMetrics
{
    public const string MeterName = "PricingService";
    private readonly Meter _meter;

    private readonly Counter<long> _priceNotFound;

    private readonly Counter<long> _handlerErrors;
    private readonly Histogram<double> _handlerDuration;

    private readonly Counter<long> _snapshotNotFound;
    private readonly Counter<long> _snapshotOutdated;

    public PricingServiceMetrics()
    {
        _meter = new Meter(MeterName);

        _priceNotFound = _meter.CreateCounter<long>(
            "pricing.prices.not_found",
            description: "Price not found for a given product variant and vendor");

        _handlerErrors = _meter.CreateCounter<long>(
            "pricing.handlers.errors");

        _handlerDuration = _meter.CreateHistogram<double>(
            "pricing.handlers.duration",
            unit: "ms");

        _snapshotNotFound = _meter.CreateCounter<long>(
            "pricing.snapshots.not_found",
            description: "Snapshot missing when projection arrived — possible race condition");

        _snapshotOutdated = _meter.CreateCounter<long>(
            "pricing.snapshots.outdated",
            description: "Projection skipped because version is outdated");
    }

    public void RecordPriceNotFound() =>
        _priceNotFound.Add(1);

    public void RecordHandlerError(string handlerName, string handlerType) =>
        _handlerErrors.Add(1,
            new KeyValuePair<string, object?>("handler", handlerName),
            new KeyValuePair<string, object?>("type", handlerType));

    public void RecordHandlerDuration(double ms, string handlerName, string handlerType) =>
        _handlerDuration.Record(ms,
            new KeyValuePair<string, object?>("handler", handlerName),
            new KeyValuePair<string, object?>("type", handlerType));

    public void RecordSnapshotNotFound(string snapshotType) =>
        _snapshotNotFound.Add(1, new KeyValuePair<string, object?>("type", snapshotType));

    public void RecordSnapshotOutdated(string handlerName) =>
        _snapshotOutdated.Add(1, new KeyValuePair<string, object?>("handler", handlerName));

    public void Dispose() => _meter.Dispose();
}
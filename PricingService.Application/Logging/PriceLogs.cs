using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Logging;

public static partial class PriceLogs
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Could not archive Price {PriceId}: {Reason}. Product may already be archived.")]
    public static partial void PriceArchiveFailed(
        this ILogger logger, Guid priceId, string reason);
}

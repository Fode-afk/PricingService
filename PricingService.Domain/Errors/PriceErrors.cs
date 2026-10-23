using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class PriceErrors
{
    public static Error AlreadyExists() =>
        Error.AlreadyExists(PriceErrorCodes.AlreadyExists,
            "Price already exists.");

    public static Error NotFound() =>
        Error.NotFound(PriceErrorCodes.NotFound,
            "Price not found.");

    public static Error NoScheduledPrice() => 
        Error.NotFound(PriceErrorCodes.NotFound,
            "No scheduled price found.");

    public static Error NoActivePrice() =>
        Error.NotFound(PriceErrorCodes.NoActivePrice,
            "No active price found.");

    public static Error CurrencyMismatch() =>
        Error.InvalidArgument(PriceErrorCodes.CurrencyMismatch,
            "Currency mismatch.");
}

public static class PriceErrorCodes
{
    public const string AlreadyExists = "Price.AlreadyExists";
    public const string NotFound = "Price.NotFound";
    public const string NoScheduledPrice = "Price.NoScheduledPrice";
    public const string NoActivePrice = "Price.NoActivePrice";
    public const string CurrencyMismatch = "Price.CurrencyMismatch";
    public const string InvalidId = "Price.InvalidId";
    public const string InvalidPagination = "Price.InvalidPagination";
}
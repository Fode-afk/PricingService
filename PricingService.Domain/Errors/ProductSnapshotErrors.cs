using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class ProductSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(ProductSnapshotErrorCodes.NotFound,
            "ProductSnapshot not found.");

    public static Error CannotEditOperationalData() =>
        Error.InvalidArgument(ProductSnapshotErrorCodes.CannotEditOperationalData,
            "Cannot edit operational data for ProductSnapshot.");

    public static Error DoesNotBelongToVendor() =>
        Error.InvalidArgument(ProductSnapshotErrorCodes.DoesNotBelongToVendor,
            "ProductSnapshot does not belong to the vendor.");

    public static Error AlreadyExists() =>
        Error.AlreadyExists(ProductSnapshotErrorCodes.AlreadyExists,
            "ProductSnapshot already exists.");

    public static Error MarketNotFound() =>
        Error.NotFound(ProductSnapshotErrorCodes.MarketNotFound,
            "Market not found.");
}

public static class ProductSnapshotErrorCodes
{
    public const string NotFound = "ProductSnapshot.NotFound";
    public const string CannotEditOperationalData = "ProductSnapshot.CannotEditOperationalData";
    public const string DoesNotBelongToVendor = "ProductSnapshot.DoesNotBelongToVendor";
    public const string AlreadyExists = "ProductSnapshot.AlreadyExists";
    public const string MarketNotFound = "ProductSnapshot.MarketNotFound";
}
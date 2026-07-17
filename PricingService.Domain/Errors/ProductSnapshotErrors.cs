using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class ProductSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(ProductSnapshotErrorCodes.NotFound,
            "ProductSnapshot not found.");

    public static Error CannotModify() =>
        Error.InvalidArgument(ProductSnapshotErrorCodes.CannotModify,
            "Cannot modify ProductSnapshot.");

    public static Error DoesNotBelongToVendor() =>
        Error.InvalidArgument(ProductSnapshotErrorCodes.DoesNotBelongToVendor,
            "ProductSnapshot does not belong to the vendor.");

    public static Error AlreadyExists() =>
        Error.AlreadyExists(ProductSnapshotErrorCodes.AlreadyExists,
            "ProductSnapshot already exists.");
}

public static class ProductSnapshotErrorCodes
{
    public const string NotFound = "ProductSnapshot.NotFound";
    public const string CannotModify = "ProductSnapshot.CannotModify";
    public const string DoesNotBelongToVendor = "ProductSnapshot.DoesNotBelongToVendor";
    public const string AlreadyExists = "ProductSnapshot.AlreadyExists";
}
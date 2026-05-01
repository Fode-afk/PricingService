using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class ProductSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(ProductSnapshotErrorCodes.NotFound);
    public static Error InvalidVendor() => Error.Unauthenticated(ProductSnapshotErrorCodes.InvalidVendor);
    public static Error CannotModifyWhenArchived() => Error.InvalidArgument(ProductSnapshotErrorCodes.CannotModifyWhenArchived);
    public static Error AlreadyExists() => Error.AlreadyExists(ProductSnapshotErrorCodes.AlreadyExists);
}

public static class ProductSnapshotErrorCodes
{
    public const string NotFound = "ProductSnapshot.NotFound";
    public const string InvalidVendor = "ProductSnapshot.InvalidVendor";
    public const string CannotModifyWhenArchived = "ProductSnapshot.CannotModifyWhenArchived";
    public const string AlreadyExists = "ProductSnapshot.AlreadyExists";
}
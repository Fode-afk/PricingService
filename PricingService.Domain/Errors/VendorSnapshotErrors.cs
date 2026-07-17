using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class VendorSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(VendorSnapshotErrorCodes.NotFound,
            "VendorSnapshot not found.");

    public static Error CannotModify() =>
        Error.Unauthenticated(VendorSnapshotErrorCodes.CannotModify,
            "Cannot modify VendorSnapshot.");
}

public static class VendorSnapshotErrorCodes
{
    public const string NotFound = "VendorSnapshot.NotFound";
    public const string CannotModify = "VendorSnapshot.CannotModify";
}
using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class ProductVariantSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(ProductVariantSnapshotErrorCodes.NotFound,
            "ProductVariantSnapshot not found.");
}

public static class ProductVariantSnapshotErrorCodes
{
    public const string NotFound = "ProductVariantSnapshot.NotFound";
}
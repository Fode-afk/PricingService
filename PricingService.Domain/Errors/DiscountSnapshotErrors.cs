using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class DiscountSnapshotErrors
{
    public static Error InvalidPercentage() => Error.InvalidArgument(DiscountSnapshotErrorCodes.InvalidPercentage);
    public static Error InvalidFixedPrice() => Error.InvalidArgument(DiscountSnapshotErrorCodes.InvalidFixedPrice);
    public static Error InvalidDateRange() => Error.InvalidArgument(DiscountSnapshotErrorCodes.InvalidDateRange);
}

public static class DiscountSnapshotErrorCodes
{
    public const string InvalidPercentage = "DiscountSnapshot.InvalidPercentage";
    public const string InvalidFixedPrice = "DiscountSnapshot.InvalidFixedPrice";
    public const string InvalidDateRange = "DiscountSnapshot.InvalidDateRange";
}
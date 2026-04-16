using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class DiscountErrors
{
    public static Error DiscountMustBeLowerThanBase() => Error.InvalidArgument(DiscountErrorCodes.DiscountMustBeLowerThanBase);
    public static Error InvalidDiscountPeriod() => Error.InvalidArgument(DiscountErrorCodes.InvalidDiscountPeriod);
    public static Error PriceIsRequired() => Error.InvalidArgument(DiscountErrorCodes.PriceIsRequired);
    public static Error InvalidPercentage() => Error.InvalidArgument(DiscountErrorCodes.InvalidPercentage);
}

public static class DiscountErrorCodes
{
    public const string DiscountMustBeLowerThanBase = "Discount.DiscountMustBeLowerThanBase";
    public const string InvalidDiscountPeriod = "Discount.InvalidDiscountPeriod";
    public const string PriceIsRequired = "Discount.PriceIsRequired";
    public const string InvalidPercentage = "Discount.InvalidPercentage";
}
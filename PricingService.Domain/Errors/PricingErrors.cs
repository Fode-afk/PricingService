using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class PricingErrors
{
    public static Error BasePriceIsRequired() => Error.InvalidArgument(PricingErrorCodes.BasePriceIsRequired);
    public static Error InvalidDiscountForNewPrice() => Error.InvalidArgument(PricingErrorCodes.InvalidDiscountForNewPrice);
    public static Error DiscountIsRequired() => Error.InvalidArgument(PricingErrorCodes.DiscountIsRequired);
    public static Error InvalidDiscount() => Error.InvalidArgument(PricingErrorCodes.InvalidDiscount);
    public static Error DiscountAlreadySet() => Error.InvalidArgument(PricingErrorCodes.DiscountAlreadySet);
    public static Error NotFound() => Error.NotFound(PricingErrorCodes.NotFound);
}

public static class PricingErrorCodes
{
    public const string BasePriceIsRequired = "Pricing.BasePriceIsRequired";
    public const string InvalidDiscountForNewPrice = "Pricing.InvalidDiscountForNewPrice";
    public const string DiscountIsRequired = "Pricing.DiscountIsRequired";
    public const string InvalidDiscount = "Pricing.InvalidDiscount";
    public const string DiscountAlreadySet = "Pricing.DiscountAlreadySet";
    public const string NotFound = "Pricing.NotFound";
}
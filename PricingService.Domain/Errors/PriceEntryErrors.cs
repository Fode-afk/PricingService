using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class PriceEntryErrors
{
    public static Error InvalidTerms() =>
        Error.InvalidArgument(PriceEntryErrorCodes.InvalidTerms,
            "Invalid terms for PriceEntry.");
}

public static class PriceEntryErrorCodes
{
    public const string InvalidTerms = "PriceEntry.InvalidTerms";
}

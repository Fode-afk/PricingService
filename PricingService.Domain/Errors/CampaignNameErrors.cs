using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class CampaignNameErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(CampaignNameErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(CampaignNameErrorCodes.TooLong);
}

public static class CampaignNameErrorCodes
{
    public const string NullOrEmpty = "CampaignName.NullOrEmpty";
    public const string TooLong = "CampaignName.TooLong";
}
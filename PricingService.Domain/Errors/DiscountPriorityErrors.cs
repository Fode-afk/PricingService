using migApp.Shared.Results;

namespace PricingService.Domain.Errors;

public static class DiscountPriorityErrors
{
    public static Error InvalidPriority() => Error.InvalidArgument(DiscountPriorityErrorCodes.InvalidPriority);
}

public static class DiscountPriorityErrorCodes  
{
    public const string InvalidPriority = "DiscountPriority.InvalidPriority";
}


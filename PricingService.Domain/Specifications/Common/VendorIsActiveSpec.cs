using migApp.Shared.Results;
using PricingService.Domain.Abstractions;
using PricingService.Domain.Errors;
using PricingService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.Specifications.Common;

public sealed class VendorIsActiveSpec<T> : Specification<T>
    where T : IVendorContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.VendorIsActive)
            return Fail(VendorSnapshotErrors.CannotModify());

        return Ok();
    }
}
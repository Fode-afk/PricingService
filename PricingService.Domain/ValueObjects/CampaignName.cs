using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using PricingService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Domain.ValueObjects;

public sealed class CampaignName : ValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    private CampaignName(string value)
    {
        Value = value;
    }

    public static IResult<CampaignName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<CampaignName>(CampaignNameErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<CampaignName>(CampaignNameErrors.TooLong());

        return Ok(new CampaignName(value));
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CampaignName campaignName) => campaignName.Value;
}

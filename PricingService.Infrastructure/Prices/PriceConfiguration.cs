using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Models;
using PricingService.Domain.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PricingService.Infrastructure.Prices;

internal sealed class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ProductId);

        builder.Property(x => x.BasePrice)
            .HasConversion(
                basePrice => basePrice.Amount,
                value => Money.Create(value, Currency.USD).Value)
            .IsRequired();

        builder.Property(x => x.Discount)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<DiscountSnapshot>(v, JsonOptions));

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}

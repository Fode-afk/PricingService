using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Models;

namespace PricingService.Infrastructure.Data.Configurations;

public sealed class PriceEntryConfiguration : IEntityTypeConfiguration<PriceEntry>
{
    public void Configure(EntityTypeBuilder<PriceEntry> builder)
    {
        builder.ToTable("PriceEntries", Schemas.PricesWrite);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EffectiveFrom).IsRequired();
        builder.Property(e => e.EffectiveTo);

        builder.Property(e => e.Amount)
            .HasPrecision(18, 4)
            .HasConversion(
                amount => amount.Amount,
                value => Money.Create(value, Currency.USD).Value);

        builder.HasIndex(e => e.PriceId);

        builder.Property(x => x.RowVersion)
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}

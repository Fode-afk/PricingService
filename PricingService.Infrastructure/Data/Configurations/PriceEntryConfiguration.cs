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
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.EffectiveFrom).IsRequired();
        builder.Property(e => e.EffectiveTo);

        builder.ComplexProperty(e => e.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(p => p.Currency)
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(
                    currency => currency.Code,
                    value => Currency.Create(value).Value);
        });

        builder.Property(e => e.EffectiveFrom).IsRequired();

        builder.HasIndex(e => e.PriceId);

        builder.Property(x => x.RowVersion)
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}

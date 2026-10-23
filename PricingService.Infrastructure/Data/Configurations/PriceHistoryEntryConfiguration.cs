using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Models;

namespace PricingService.Infrastructure.Data.Configurations;

public sealed class PriceHistoryEntryConfiguration : IEntityTypeConfiguration<PriceHistoryEntry>
{
    public void Configure(EntityTypeBuilder<PriceHistoryEntry> builder)
    {
        builder.ToTable("PriceHistoryEntries", Schemas.PricesWrite);

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

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

        builder.Property(h => h.ReasonNote)
            .HasMaxLength(500);

        builder.Property(h => h.Source)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.CreatedAt).IsRequired();

        builder.HasIndex(h => h.PriceId);

        builder.Property(x => x.RowVersion)
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}

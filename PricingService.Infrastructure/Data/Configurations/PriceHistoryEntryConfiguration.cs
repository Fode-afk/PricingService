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

        builder.Property(e => e.Amount)
            .HasPrecision(18, 4)
            .HasConversion(
                amount => amount.Amount,
                value => Money.Create(value, Currency.USD).Value);

        builder.Property(h => h.ReasonNote)
            .HasMaxLength(500);

        builder.Property(h => h.Source)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(h => h.PriceId);

        builder.Property(x => x.RowVersion)
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Snapshots;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class VendorSnapshotConfiguration : IEntityTypeConfiguration<VendorSnapshot>
{
    public void Configure(EntityTypeBuilder<VendorSnapshot> builder)
    {
        builder.ToTable("VendorSnapshots", Schemas.PricesWrite);

        builder.HasKey(v => v.VendorId);

        builder.Property(v => v.BaseCurrency)
            .HasMaxLength(Currency.CodeLength)
            .HasConversion(
                currency => currency.Code,
                value => Currency.Create(value).Value);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
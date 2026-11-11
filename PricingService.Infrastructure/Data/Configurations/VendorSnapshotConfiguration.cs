using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingService.Domain.Snapshots;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class VendorSnapshotConfiguration : IEntityTypeConfiguration<VendorSnapshot>
{
    public void Configure(EntityTypeBuilder<VendorSnapshot> builder)
    {
        builder.ToTable("VendorSnapshots", Schemas.PricesWrite);

        builder.HasKey(v => v.VendorId);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
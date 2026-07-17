using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingService.Domain.Snapshots;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class ProductSnapshotConfiguration : IEntityTypeConfiguration<ProductSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductSnapshot> builder)
    {
        builder.ToTable("ProductSnapshots", Schemas.PricesWrite);

        builder.HasKey(x => x.ProductId);

        builder.HasIndex(x => x.VendorId);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
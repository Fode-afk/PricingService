using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingService.Domain.Snapshots;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantSnapshotConfiguration : IEntityTypeConfiguration<ProductVariantSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductVariantSnapshot> builder)
    {
        builder.ToTable("ProductVariantSnapshots", Schemas.PricesWrite);

        builder.HasKey(x => x.ProductVariantId);
        builder.HasIndex(x => x.ProductId);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}

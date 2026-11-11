using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;
using PricingService.Domain.Snapshots;
using PricingService.Infrastructure.Data.Converters;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class ProductSnapshotConfiguration : IEntityTypeConfiguration<ProductSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductSnapshot> builder)
    {
        builder.ToTable("ProductSnapshots", Schemas.PricesWrite);

        builder.HasKey(x => x.ProductId);

        builder.HasIndex(x => x.VendorId);

        builder.PrimitiveCollection(p => p.Markets)
            .ElementType()
            .HasConversion(new MarketConverter())
            .HasMaxLength(Market.CodeLength);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
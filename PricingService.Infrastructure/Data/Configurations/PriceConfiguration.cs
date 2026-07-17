using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingService.Domain.Models;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.ToTable("Prices", Schemas.PricesWrite);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ProductVariantId).IsUnique();
        builder.HasIndex(x => x.VendorId);

        builder.HasMany(p => p.Entries)
            .WithOne()
            .HasForeignKey("PriceId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.History)
            .WithOne()
            .HasForeignKey("PriceId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Entries)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}

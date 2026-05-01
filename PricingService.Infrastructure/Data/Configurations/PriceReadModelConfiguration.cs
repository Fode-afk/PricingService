using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PricingService.Domain.Models;

namespace PricingService.Infrastructure.Data.Configurations;

internal sealed class PriceReadModelConfiguration : IEntityTypeConfiguration<PriceReadModel>
{
    public void Configure(EntityTypeBuilder<PriceReadModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.BasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CurrentPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Percentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.FixedPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.AmountOff)
            .HasPrecision(18, 2);

        builder.Property(x => x.CampaignName)
            .HasMaxLength(256);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.ProductId).IsUnique();
        builder.HasIndex(x => x.DiscountId);
        builder.HasIndex(x => x.CurrentPrice);

        builder.Property<byte[]>("RowVersion")
          .IsRowVersion()
          .IsConcurrencyToken();
    }
}

using PricingService.Domain.Snapshots;

namespace PricingService.Infrastructure.Data.Seeds;

internal sealed class ProductVariantSnapshotSeed
{
    public static readonly List<ProductVariantSnapshot> Data =
    [
        // =========================
        // PRODUCT 1 — MULTIPLE VARIANTS
        // =========================

        new()
        {
            ProductVariantId = Guid.Parse("a1200000-0000-0000-0000-000000000001"),
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000001"),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-25),
            Version = 1
        },

        new()
        {
            ProductVariantId = Guid.Parse("a1200000-0000-0000-0000-000000000002"),
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000001"),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-10),
            Version = 3
        },

        new()
        {
            ProductVariantId = Guid.Parse("a1200000-0000-0000-0000-000000000003"),
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000001"),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-2),
            Version = 5
        },

        // =========================
        // PRODUCT 2 — SINGLE VARIANT
        // =========================

        new()
        {
            ProductVariantId = Guid.Parse("a2200000-0000-0000-0000-000000000001"),
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000002"),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-8),
            Version = 2
        },

        // =========================
        // PRODUCT 3 — MULTIPLE VARIANTS
        // =========================

        new()
        {
            ProductVariantId = Guid.Parse("a3200000-0000-0000-0000-000000000001"),
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000003"),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-5),
            Version = 1
        },

        new()
        {
            ProductVariantId = Guid.Parse("a3200000-0000-0000-0000-000000000002"),
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000003"),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            Version = 2
        }
    ];
}

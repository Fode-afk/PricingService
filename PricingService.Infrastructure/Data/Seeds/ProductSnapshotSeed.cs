using PricingService.Domain.Snapshots;

namespace PricingService.Infrastructure.Data.Seeds;

internal sealed class ProductSnapshotSeed
{
    public static readonly List<ProductSnapshot> Data =
    [
        // =========================
        // ACTIVE VENDORS — EDITABLE
        // =========================

        new()
        {
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000001"),
            VendorId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
            CanEditOperationalData = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            Version = 1
        },

        new()
        {
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000002"),
            VendorId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
            CanEditOperationalData = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-10),
            Version = 3
        },

        new()
        {
            ProductId = Guid.Parse("a1100000-0000-0000-0000-000000000003"),
            VendorId = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
            CanEditOperationalData = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-5),
            Version = 2
        },

        // =========================
        // OPERATIONAL DATA LOCKED
        // =========================

        new()
        {
            ProductId = Guid.Parse("b2100000-0000-0000-0000-000000000001"),
            VendorId = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-15),
            Version = 4
        },

        new()
        {
            ProductId = Guid.Parse("b2100000-0000-0000-0000-000000000002"),
            VendorId = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-2),
            Version = 5
        },

        // =========================
        // INACTIVE / UNVERIFIED VENDORS
        // =========================

        new()
        {
            ProductId = Guid.Parse("c3100000-0000-0000-0000-000000000001"),
            VendorId = Guid.Parse("b2000000-0000-0000-0000-000000000001"),
            CanEditOperationalData = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-20),
            Version = 1
        },

        new()
        {
            ProductId = Guid.Parse("c3100000-0000-0000-0000-000000000002"),
            VendorId = Guid.Parse("b2000000-0000-0000-0000-000000000002"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-3),
            Version = 2
        },

        // =========================
        // BLOCKED VENDORS
        // =========================

        new()
        {
            ProductId = Guid.Parse("d4100000-0000-0000-0000-000000000001"),
            VendorId = Guid.Parse("c3000000-0000-0000-0000-000000000001"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-60),
            Version = 5
        },

        new()
        {
            ProductId = Guid.Parse("d4100000-0000-0000-0000-000000000002"),
            VendorId = Guid.Parse("c3000000-0000-0000-0000-000000000002"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            Version = 3
        },

        // =========================
        // PENDING / MODERATION
        // =========================

        new()
        {
            ProductId = Guid.Parse("e5100000-0000-0000-0000-000000000001"),
            VendorId = Guid.Parse("d4000000-0000-0000-0000-000000000001"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddHours(-12),
            Version = 1
        },

        new()
        {
            ProductId = Guid.Parse("e5100000-0000-0000-0000-000000000002"),
            VendorId = Guid.Parse("d4000000-0000-0000-0000-000000000002"),
            CanEditOperationalData = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddHours(-1),
            Version = 4
        }
    ];
}

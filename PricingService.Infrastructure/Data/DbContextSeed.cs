using PricingService.Infrastructure.Data.Seeds;

namespace PricingService.Infrastructure.Data;

internal static class DbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.VendorSnapshots.Any())
            context.AddRange(VendorSnapshotSeed.Data);

        if (!context.ProductSnapshots.Any())
            context.AddRange(ProductSnapshotSeed.Data);

        if (!context.ProductVariantSnapshots.Any())
            context.AddRange(ProductVariantSnapshotSeed.Data);

        await context.SaveChangesAsync();
    }
}
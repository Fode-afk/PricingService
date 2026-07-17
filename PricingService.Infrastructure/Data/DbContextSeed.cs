using PricingService.Infrastructure.Data.Seeds;

namespace PricingService.Infrastructure.Data;

internal static class DbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.VendorSnapshots.Any())
            context.AddRange(VendorSnapshotSeed.Data);

        await context.SaveChangesAsync();
    }
}
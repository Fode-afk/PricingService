using Microsoft.EntityFrameworkCore;
using PricingService.Domain.Models;
using PricingService.Domain.Snapshots;

namespace PricingService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Price> Prices { get; }

    DbSet<ProductSnapshot> ProductSnapshots { get; }
    DbSet<ProductVariantSnapshot> ProductVariantSnapshots { get; }
    DbSet<VendorSnapshot> VendorSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
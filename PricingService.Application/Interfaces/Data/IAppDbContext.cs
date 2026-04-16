using Microsoft.EntityFrameworkCore;
using PricingService.Domain.Models;

namespace PricingService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Price> Prices { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
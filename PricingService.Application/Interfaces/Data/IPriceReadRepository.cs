using PricingService.Domain.Models;

namespace PricingService.Application.Interfaces.Data;

public interface IPriceReadRepository
{
    Task<PriceReadModel?> GetByProductId(
        Guid productId,
        CancellationToken cancellationToken = default);
}
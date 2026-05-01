using PricingService.Domain.DomainEvents;
using PricingService.Domain.Primitives;
using ZiggyCreatures.Caching.Fusion;

namespace PricingService.Application.Caching;

public sealed class InvalidatePriceCacheHandlers(IFusionCache cache) : IPostCommitDomainEventHandler<BasePriceUpdatedDomainEvent>
{
    public async Task Handle(BasePriceUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveByTagAsync(CacheTags.PriceByProductId(notification.ProductId), token: cancellationToken);
}

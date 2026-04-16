using PricingService.Domain.Primitives;

namespace PricingService.Infrastructure.DomainEvents;

public interface IDomainEventsDispatcher
{
    Task DispatchPreCommitDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    Task DispatchPostCommitDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

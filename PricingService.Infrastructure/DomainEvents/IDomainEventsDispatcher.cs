using migApp.Shared.Domain.Primitives;

namespace PricingService.Infrastructure.DomainEvents;

internal interface IDomainEventsDispatcher
{
    Task DispatchPreCommitDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    Task DispatchPostCommitDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
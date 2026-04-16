using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Images;

namespace PricingService.Infrastructure.Messaging.IntegrationEvents.Handlers;

//public sealed class VendorAvatarDeletedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<VendorAvatarDeletedDomainEvent>
//{
//    public Task Handle(VendorAvatarDeletedDomainEvent notification, CancellationToken cancellationToken) =>
//        publish.Publish(new ImageDeletedIntegrationEvent(
//            notification.VendorId.ToString(),
//            notification.OldAvatarUrl.Url), cancellationToken);
//}
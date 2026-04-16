//using MassTransit;
//using MediatR;
//using migApp.Shared.Enums.Image;
//using migApp.Shared.Messaging.Events.Images;
//using VendorService.Application.Features.Commands.UpdateVendorAvatar;
//using VendorService.Application.Features.Commands.UpdateVendorBanner;
//
//namespace PricingService.Infrastructure.Messaging.Consumers;
//
//public sealed class ImageUploadedConsumer(IMediator mediator) : IConsumer<ImageUploadedEvent>
//{
//    public Task Consume(ConsumeContext<ImageUploadedEvent> context)
//    {
//        if (context.Message.ImageType == ImageType.AVATAR_IMAGE)
//            return mediator.Send(
//                new UpdateVendorAvatarCommand(context.Message.Id, context.Message.Url),
//                context.CancellationToken);
//        else if (context.Message.ImageType == ImageType.BANNER_IMAGE)
//            return mediator.Send(
//                new UpdateVendorBannerCommand(context.Message.Id, context.Message.Url),
//                context.CancellationToken);
//        else
//            return Task.CompletedTask;
//    }
//}

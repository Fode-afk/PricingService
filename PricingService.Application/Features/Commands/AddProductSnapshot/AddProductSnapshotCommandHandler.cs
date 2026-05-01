using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using PricingService.Application.Interfaces.Data;
using PricingService.Domain.Errors;
using PricingService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace PricingService.Application.Features.Commands.AddProductSnapshot;

public sealed class AddProductSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<AddProductSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductSnapshots
            .AnyAsync(x => x.ProductId == request.ProductId, cancellationToken);

        if (exists)
            return Fail(ProductSnapshotErrors.AlreadyExists());

        var productSnapshot = new ProductSnapshot
        {
            ProductId = request.ProductId,
            VendorId = request.VendorId,
            ProductCardId = request.ProductCardId,
            Status = ProductCardStatus.Draft
        };

        context.ProductSnapshots.Add(productSnapshot);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
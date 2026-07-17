using MediatR;
using Microsoft.EntityFrameworkCore;
using PricingService.Application.Interfaces.Data;

namespace PricingService.Application.Features.IntegrationEventHandlers.ProductSnapshot.DeleteProductSnapshot;

public sealed class DeleteProductSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<DeleteProductSnapshotCommand>
{
    public async Task Handle(DeleteProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductSnapshots
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);
        if (snapshot is null)
            return;

        context.ProductSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}
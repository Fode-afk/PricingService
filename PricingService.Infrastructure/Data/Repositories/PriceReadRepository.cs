using Dapper;
using PricingService.Application.Interfaces.Data;
using PricingService.Domain.Models;

namespace PricingService.Infrastructure.Data.Repositories;

internal sealed class PriceReadRepository(IDbConnectionFactory connectionFactory) : IPriceReadRepository
{
    public async Task<PriceReadModel?> GetByProductId(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP 1
                [Id],
                [ProductId],
                [BasePrice],
                [CurrentPrice],

                [DiscountId],
                [DiscountType],
                [Percentage],
                [FixedPrice],
                [AmountOff],

                [CampaignName],
                [Priority],
                [IsStackable],

                [DiscountStart],
                [DiscountEnd],

                [CreatedAt],
                [UpdatedAt]
            FROM [prices].[PriceReadModels]
            WHERE [ProductId] = @ProductId
            """;

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { ProductId = productId },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<PriceReadModel>(command);
    }
}

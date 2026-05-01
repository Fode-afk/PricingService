using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PricingService.Application.Interfaces.Data;
using System.Data;

namespace PricingService.Infrastructure.Data;

internal sealed class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Database"));
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

using System.Text.Json;
using Microsoft.Data.SqlClient;
using MoviDB.Infrastructure.Serialization;

namespace MoviDB.Infrastructure.Database;

/// <summary>
/// Creates SQL Server connections using configuration provided by an IConfigLoader.
/// </summary>
public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(DatabaseConnectionConfig configuration)
    {
        _connectionString = BuildConnectionString(configuration);
    }

    /// <summary>
    /// Creates and opens a new SQL connection synchronously.
    /// </summary>
    public SqlConnection CreateOpenConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Creates and opens a new SQL connection asynchronously.
    /// </summary>
    public async Task<SqlConnection> CreateOpenConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    private static string BuildConnectionString(DatabaseConnectionConfig configuration)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = configuration.Server,
            InitialCatalog = configuration.Database,
            UserID = configuration.UserId,
            Password = configuration.Password,
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }
}
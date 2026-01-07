using System.IO;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace MoviDB.Infrastructure.Database;

/// <summary>
/// Creates SQL Server connections using configuration loaded from JSON.
/// </summary>
public sealed class SqlConnectionFactory
{
    private readonly string connectionString;

    /// <summary>
    /// Initializes the factory and builds the connection string once.
    /// </summary>
    public SqlConnectionFactory(string configurationFilePath)
    {
        var configuration = LoadDatabaseConfiguration(configurationFilePath);
        connectionString = BuildConnectionString(configuration);
    }

    /// <summary>
    /// Creates and opens a new SQL connection.
    /// </summary>
    public SqlConnection CreateOpenConnection()
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    private static DatabaseConfiguration LoadDatabaseConfiguration(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"database configuration file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);
        var configuration = JsonSerializer.Deserialize<DatabaseConfiguration>(json);

        if (configuration == null
            || string.IsNullOrWhiteSpace(configuration.Server)
            || string.IsNullOrWhiteSpace(configuration.Database)
            || string.IsNullOrWhiteSpace(configuration.UserId)
            || string.IsNullOrWhiteSpace(configuration.Password))
        {
            throw new InvalidOperationException("database configuration is invalid");
        }

        return configuration;
    }

    private static string BuildConnectionString(DatabaseConfiguration configuration)
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

    private sealed class DatabaseConfiguration
    {
        public string Server { get; init; } = string.Empty;
        public string Database { get; init; } = string.Empty;
        public string UserId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}

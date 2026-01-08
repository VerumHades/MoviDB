namespace MoviDB.Infrastructure.Serialization;

/// <summary>
/// Interface to provide database configuration.
/// </summary>
public interface IConfigLoader
{
    DatabaseConnectionConfig LoadConfiguration();
}
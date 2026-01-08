using System.Text.Json;

namespace MoviDB.Infrastructure.Serialization;

/// <summary>
/// Implementation that loads configuration from a JSON file.
/// </summary>
public sealed class JsonFileConfigLoader : IConfigLoader
{
    private readonly string _filePath;

    public JsonFileConfigLoader(string filePath)
    {
        _filePath = filePath;
    }

    public DatabaseConnectionConfig LoadConfiguration()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"Database configuration file not found: {_filePath}");
        }

        var json = File.ReadAllText(_filePath);
        var config = JsonSerializer.Deserialize<DatabaseConnectionConfig>(json);

        if (config == null
            || string.IsNullOrWhiteSpace(config.Server)
            || string.IsNullOrWhiteSpace(config.Database)
            || string.IsNullOrWhiteSpace(config.UserId)
            || string.IsNullOrWhiteSpace(config.Password))
        {
            throw new InvalidOperationException("Database configuration is invalid");
        }

        return config;
    }
}

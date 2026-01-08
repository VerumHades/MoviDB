namespace MoviDB.Infrastructure.Serialization;

public sealed class DatabaseConnectionConfig
{
    public string Server { get; init; } = string.Empty;
    public string Database { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
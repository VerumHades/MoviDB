using System.Data;
using Microsoft.Data.SqlClient;
using MoviDB.Infrastructure.Database;

namespace MoviDB.Infrastructure;

/// <summary>
/// Executes SQL commands in autocommit mode using a connection factory for thread safety.
/// </summary>
public sealed class SqlServerAutocommitExecutor : ISqlExecutor
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SqlServerAutocommitExecutor(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public Task ExecuteNonQueryAsync(string sql, IReadOnlyDictionary<string, object> parameters) =>
        ExecuteWithCommandAsync(async command => await command.ExecuteNonQueryAsync(), sql, parameters);

    public Task<T> ExecuteScalarAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters) =>
        ExecuteWithCommandAsync(async command =>
        {
            var result = await command.ExecuteScalarAsync();
            if (result == null || result is DBNull)
                throw new InvalidOperationException("Scalar query returned null");

            return (T)Convert.ChangeType(result, typeof(T));
        }, sql, parameters);

    public Task<List<T>> QueryAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters, Func<IDataRecord, T> map) =>
        ExecuteWithCommandAsync(async command =>
        {
            var results = new List<T>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }
            return results;
        }, sql, parameters);

    /// <summary>
    /// Centralized helper: creates connection + command, executes the provided function, disposes everything.
    /// </summary>
    private async Task<T> ExecuteWithCommandAsync<T>(
        Func<SqlCommand, Task<T>> executor,
        string sql,
        IReadOnlyDictionary<string, object> parameters)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = CreateCommand(connection, sql, parameters);
        return await executor(command);
    }

    /// <summary>
    /// Creates a SqlCommand with parameters for the given connection.
    /// </summary>
    private static SqlCommand CreateCommand(SqlConnection connection, string sql, IReadOnlyDictionary<string, object> parameters)
    {
        var command = new SqlCommand(sql, connection);
        

        foreach (var (key, value) in parameters)
        {
            command.Parameters.AddWithValue(key, value);
        }
        
        return command;
    }
}

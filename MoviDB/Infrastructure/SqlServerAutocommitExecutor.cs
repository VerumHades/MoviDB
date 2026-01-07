using System.Data;
using Microsoft.Data.SqlClient;

namespace MoviDB.Infrastructure;

/// <summary>
/// Executes SQL commands in autocommit mode.
/// </summary>
public sealed class SqlServerAutocommitExecutor : ISqlExecutor
{
    private readonly SqlConnection connection;

    public SqlServerAutocommitExecutor(SqlConnection connection)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task ExecuteNonQueryAsync(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        using var command = CreateCommand(sql, parameters);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        using var command = CreateCommand(sql, parameters);
        var result = await command.ExecuteScalarAsync();

        if (result == null || result is DBNull)
            throw new InvalidOperationException("scalar query returned null");

        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<List<T>> QueryAsync<T>(
        string sql,
        IReadOnlyDictionary<string, object> parameters,
        Func<IDataRecord, T> map)
    {
        var results = new List<T>();

        using var command = CreateCommand(sql, parameters);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(map(reader));
        }

        return results;
    }

    private SqlCommand CreateCommand(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        var command = new SqlCommand(sql, connection);

        if (parameters != null)
        {
            foreach (var (key, value) in parameters)
            {
                command.Parameters.AddWithValue(key, value ?? DBNull.Value);
            }
        }

        return command;
    }
}

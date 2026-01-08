using System.Data;
using Microsoft.Data.SqlClient;

namespace MoviDB.Infrastructure;

/// <summary>
/// Executes SQL commands bound to an existing transaction.
/// </summary>
public sealed class SqlServerTransactionalExecutor : ISqlExecutor
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;

    public SqlServerTransactionalExecutor(SqlConnection connection, SqlTransaction transaction)
    {
        this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this._transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }

    public async Task ExecuteNonQueryAsync(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        await using var command = CreateCommand(sql, parameters);
        await command.ExecuteNonQueryAsync();
    }
//
    public async Task<T> ExecuteScalarAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        await using var command = CreateCommand(sql, parameters);
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

        await using var command = CreateCommand(sql, parameters);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(map(reader));
        }

        return results;
    }

    private SqlCommand CreateCommand(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        var command = new SqlCommand(sql, _connection, _transaction);

        foreach (var (key, value) in parameters)
        {
            command.Parameters.AddWithValue(key, value);
        }

        return command;
    }
}

using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace MoviDB.Infrastructure;

public class SqlServerExecutor : ISqlExecutor
{
    private readonly SqlConnection _connection;

    public SqlServerExecutor(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task ExecuteNonQueryAsync(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        using var command = new SqlCommand(sql, _connection);
        AddParameters(command, parameters);
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        using var command = new SqlCommand(sql, _connection);
        AddParameters(command, parameters);
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        return (T)(Convert.ChangeType(result, typeof(T)) ?? throw new InvalidOperationException());
    }

    public async Task ExecuteAllNonQueryInTransactionAsync(string[] sqlCommands, IReadOnlyDictionary<string, object> parameters)
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        using var transaction = _connection.BeginTransaction();
        try
        {
            foreach (var sql in sqlCommands)
            {
                using var command = new SqlCommand(sql, _connection, transaction);
                AddParameters(command, parameters);
                await command.ExecuteNonQueryAsync();
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<T>> QueryAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters, Func<IDataRecord, T> map)
    {
        var results = new List<T>();

        using var command = new SqlCommand(sql, _connection);
        AddParameters(command, parameters);
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(map(reader));
        }

        return results;
    }

    private static void AddParameters(SqlCommand command, IReadOnlyDictionary<string, object> parameters)
    {
        if (parameters == null) return;
        foreach (var kv in parameters)
        {
            command.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
        }
    }
}

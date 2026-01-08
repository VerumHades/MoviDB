using System.Data;

namespace MoviDB.Infrastructure;

public interface ISqlExecutor
{
    Task ExecuteNonQueryAsync(string sql, IReadOnlyDictionary<string, object> parameters);
    Task<T> ExecuteScalarAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters);
    Task<List<T>> QueryAsync<T>(string sql, IReadOnlyDictionary<string, object> parameters, Func<IDataRecord, T> map);
}
using MoviDB.Domain.Repositories;

namespace MoviDB.Infrastructure.Repositories;

public class SqlMediaExistenceChecker : IMediaExistenceChecker
{
    private readonly ISqlExecutor _sqlExecutor;

    public SqlMediaExistenceChecker(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }

    public async Task<bool> MediaExistsAsync(int id)
    {
        const string sql = "SELECT TOP 1 id FROM media WHERE id = @id";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = id
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, reader => reader.GetInt32(0));
        return result.Any();
    }
}

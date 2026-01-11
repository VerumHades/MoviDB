using MoviDB.Domain.Repositories;

namespace MoviDB.Infrastructure.Repositories;

public sealed class SqlUserQueryLibraryRepository(ISqlExecutor executor): Repository(executor), IUserLibraryQueryRepository
{
    public async Task<bool> LibraryEntryExistsAsync(int userId, int mediaId)
    {
        const string sql = @"
            SELECT COUNT(1)
            FROM library_entry
            WHERE user_id = @userId AND media_id = @mediaId;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@userId"] = userId,
            ["@mediaId"] = mediaId
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader => reader.GetInt32(0));
        return rows.FirstOrDefault() > 0;
    }
}


public sealed class SqlUserLibraryCommandRepository(ISqlExecutor executor) : Repository(executor), IUserLibraryCommandRepository
{
    public async Task AddLibraryEntryAsync(int userId, int mediaId)
    {
        const string sql = @"
            INSERT INTO library_entry (media_id, user_id, watched)
            VALUES (@mediaId, @userId, 0);
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@mediaId"] = mediaId,
            ["@userId"] = userId
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
    }

    public async Task MarkWatchedStateAsync(int userId, int mediaId, bool isWatched)
    {
        const string sql = @"
            UPDATE library_entry
            SET watched = {isWatched ? 1 : 0}
            WHERE user_id = @userId AND media_id = @mediaId;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@userId"] = userId,
            ["@mediaId"] = mediaId
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
    }
}
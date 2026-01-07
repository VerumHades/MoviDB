using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Repositories;

namespace MoviDB.Infrastructure.Repositories;

public class SqlGenreQueryRepository(ISqlExecutor executor): Repository(executor), IGenreQueryRepository
{
    public async Task<bool> ExistsByNameAsync(string name)
    {
        const string sql = "SELECT TOP 1 id FROM genre WHERE name = @name";

        var parameters = new Dictionary<string, object>
        {
            ["@name"] = name
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, reader => reader.GetInt32(0));
        return result.Any();
    }

    public async Task<Genre> GetByNameAsync(string name)
    {
        const string sql = "SELECT TOP 1 id, name FROM genre WHERE name = @name";

        var parameters = new Dictionary<string, object>
        {
            ["@name"] = name
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
            Genre.Hydrate(reader.GetInt32(0), reader.GetString(1))
        );

        var genre = result.FirstOrDefault();
        if (genre is null)
            throw new KeyNotFoundException($"Genre '{name}' not found.");

        return genre;
    }
}

public class SqlGenreCommandRepository(ISqlExecutor executor): Repository(executor), IGenreCommandRepository
{
    public async Task<Genre> CreateAsync(string name)
    {
        const string sql = @"
            INSERT INTO genre (name)
            OUTPUT INSERTED.id
            VALUES (@name);
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@name"] = name
        };

        var newId = await _sqlExecutor.ExecuteScalarAsync<int>(sql, parameters);
        return Genre.Hydrate(newId, name);
    }
}
using System.Text;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.User;
using MoviDB.Domain.Repositories;
using MoviDB.Domain.ValueObjects;
using MoviDB.Infrastructure.Database;

namespace MoviDB.Infrastructure.Repositories;

public sealed class SqlUserQueryRepository(ISqlExecutor sqlExecutor) : Repository(sqlExecutor), IUserQueryRepository
{


    public async Task<bool> ExistsByNameAsync(string username)
    {
        const string sql = "SELECT COUNT(1) FROM [user] WHERE username = @username";

        var parameters = new Dictionary<string, object>
        {
            ["@username"] = username
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader => reader.GetInt32(0));
        return rows.FirstOrDefault() > 0;
    }

    public async Task<User> GetByNameAsync(string username)
    {
        const string sql = "SELECT id, username, password_hash, role FROM [user] WHERE username = @username";

        var parameters = new Dictionary<string, object>
        {
            ["@username"] = username
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string userName = reader.GetString(1);
            string passwordHash = reader.GetString(2);
            string roleStr = reader.GetString(3);
            var role = Enum.Parse<UserRole>(roleStr, ignoreCase: true);

            return User.Hydrate(id, username, passwordHash, role);
        });

        var user = rows.FirstOrDefault();

        if (user == null)
            throw new KeyNotFoundException($"User '{username}' not found.");

        return user;
    }
    
    public async Task<(UserProjection[],UserCursor)> GetNextBatchOfAllAsync(int batchSize,UserCursor? cursor = null){
        var (filter, parameters) = FilterMapper.BuildBatchClause(cursor?.CreatedAt, cursor?.Id);

        DateTime createdAt = new DateTime();
        var rows = await _sqlExecutor.QueryAsync($"SELECT TOP {batchSize} id, username, created_at FROM [user] {filter}", parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string username = reader.GetString(1);
           
            createdAt = reader.GetDateTime(2);
            return new UserProjection(id, username);
        });

        var nextCursor = rows.Count > 0
            ? new UserCursor(rows[^1].Id, createdAt)
            : cursor ?? new UserCursor(0, DateTime.MinValue);

        return (rows.ToArray(), nextCursor);
    }
}

public sealed class SqlUserCommandRepository(ISqlExecutor sqlExecutor) : Repository(sqlExecutor), IUserCommandRepository
{
    public async Task<User> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO [user] (username, password_hash, role)
            OUTPUT INSERTED.id, INSERTED.username, INSERTED.password_hash, INSERTED.role
            VALUES (@username, @passwordHash, @role);
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@username"] = user.Username,
            ["@passwordHash"] = user.PasswordHash,
            ["@role"] = user.Role.ToString()
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string username = reader.GetString(1);
            string passwordHash = reader.GetString(2);
            string roleStr = reader.GetString(3);
            var role = Enum.Parse<UserRole>(roleStr, ignoreCase: true);

            return User.Hydrate(id, username, passwordHash, role);
        });

        if (rows.Count == 0)
            throw new InvalidOperationException("Failed to insert user.");

        return rows[0];
    }

    public async Task<User> UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE [user]
            SET password_hash = @passwordHash
            WHERE id = @id
            OUTPUT INSERTED.id, INSERTED.username, INSERTED.password_hash, INSERTED.role;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = user.Id,
            ["@passwordHash"] = user.PasswordHash
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string username = reader.GetString(1);
            string passwordHash = reader.GetString(2);
            string roleStr = reader.GetString(3);
            var role = Enum.Parse<UserRole>(roleStr, ignoreCase: true);

            return User.Hydrate(id, username, passwordHash, role);
        });

        if (rows.Count == 0)
            throw new KeyNotFoundException($"User with ID {user.Id} not found.");

        return rows[0];
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be positive.", nameof(id));

        const string sqlDelete = @"
            DELETE FROM [user]
            WHERE id = @id;
        ";
        
        var parameters = new Dictionary<string, object>
        {
            ["@id"] = id
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sqlDelete, parameters);
    }
}
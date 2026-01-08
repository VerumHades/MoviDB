using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Repositories;

namespace MoviDB.Infrastructure.Repositories;

public class SqlReviewCommandRepository : IReviewCommandRepository
{
    private readonly ISqlExecutor _sqlExecutor;

    public SqlReviewCommandRepository(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }

    public async Task CreateAsync(Review review)
    {
        const string sql = @"
            INSERT INTO review (media_id, user_id, title, content, rating)
            OUTPUT INSERTED.id
            VALUES (@media_id, @user_id, @title, @content, @rating);
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@media_id"] = review.MediaId,
            ["@user_id"] = review.UserId,
            ["@title"] = review.Title,
            ["@content"] = review.Content,
            ["@rating"] = review.Rating
        };

        var newId = await _sqlExecutor.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task UpdateAsync(Review review)
    {
        const string sql = @"
            UPDATE review
            SET title = @title,
                content = @content,
                rating = @rating
            WHERE media_id = @media_id AND user_id = @user_id;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@media_id"] = review.MediaId,
            ["@user_id"] = review.UserId,
            ["@title"] = review.Title,
            ["@content"] = review.Content,
            ["@rating"] = review.Rating
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
    }

    public async Task RemoveAsync(int reviewId, int userId)
    {
        const string sql = @"
            DELETE FROM review
            WHERE id = @id AND user_id = @user_id;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = reviewId,
            ["@user_id"] = userId
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
    }
}

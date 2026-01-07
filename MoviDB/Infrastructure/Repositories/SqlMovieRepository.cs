
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Repositories;
using MoviDB.Domain.ValueObjects;

namespace MoviDB.Infrastructure.Repositories;

public class SqlMovieQueryRepository(ISqlExecutor executor) : Repository(executor), IMovieQueryRepository
{
    public async Task<Movie?> GetByIdAsync(int mediaId)
    {
        const string sql = "SELECT * FROM vw_movie WHERE media_id = @id";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = mediaId
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string description = reader.GetString(2);
            int genreId = reader.GetInt32(3);
            string genreName = reader.GetString(4);
            //int ratingCount = reader.GetInt32(5);
            //double ratingSum = reader.GetDouble(6);
            int durationMinutes = reader.GetInt32(7);

            return Movie.Hydrate(id, title, description, genreId, genreName, durationMinutes);
        });

        return result.FirstOrDefault();
    }

    public async Task<(MovieProjection[], MovieCursor)> GetNextBatchOfAllAsync(int batchSize,
        MovieCursor? cursor = null, MovieFilter? filter = null)
    {
        var sqlBuilder = new List<string>();
        var parameters = new Dictionary<string, object>();

        sqlBuilder.Add(@"SELECT TOP @batchSize * FROM vw_movie WHERE 1=1 ");
        parameters["@batchSize"] = batchSize;

        if (cursor != null)
        {
            sqlBuilder.Add("AND (created_at > @cursorCreated OR (created_at = @cursorCreated AND id > @cursorId))");
            parameters["@cursorCreated"] = cursor.CreatedAt;
            parameters["@cursorId"] = cursor.Id;
        }

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.TitleContains))
            {
                sqlBuilder.Add("AND title LIKE @titleContains");
                parameters["@titleContains"] = $"%{filter.TitleContains}%";
            }

            if (filter.GenreId.HasValue)
            {
                sqlBuilder.Add("AND genre_id = @genreId");
                parameters["@genreId"] = filter.GenreId.Value;
            }

            if (filter.MinRating.HasValue)
            {
                sqlBuilder.Add("AND rating >= @minRating");
                parameters["@minRating"] = filter.MinRating.Value;
            }

            if (filter.MaxRating.HasValue)
            {
                sqlBuilder.Add("AND rating <= @maxRating");
                parameters["@maxRating"] = filter.MaxRating.Value;
            }

            if (filter.MinDuration.HasValue)
            {
                sqlBuilder.Add("AND duration_minutes >= @minDuration");
                parameters["@minDuration"] = filter.MinDuration.Value;
            }

            if (filter.MaxDuration.HasValue)
            {
                sqlBuilder.Add("AND duration_minutes <= @maxDuration");
                parameters["@maxDuration"] = filter.MaxDuration.Value;
            }

            if (filter.CreatedAfter.HasValue)
            {
                sqlBuilder.Add("AND created_at >= @createdAfter");
                parameters["@createdAfter"] = filter.CreatedAfter.Value;
            }

            if (filter.CreatedBefore.HasValue)
            {
                sqlBuilder.Add("AND created_at <= @createdBefore");
                parameters["@createdBefore"] = filter.CreatedBefore.Value;
            }
        }

        sqlBuilder.Add("ORDER BY created_at, id"); // cursor ordering

        var sql = string.Join(" ", sqlBuilder);

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string description = reader.GetString(2);
            string genreName = reader.GetString(3);
            int ratingCount = reader.GetInt32(4);
            double ratingSum = reader.GetDouble(5);
            double average = ratingCount > 0 ? ratingSum / ratingCount : 0.0;
            var ratingSnapshot = new RatingSnapshot(ratingCount, average);

            return new MovieProjection(id, title, description, genreName, ratingSnapshot);
        });

        var nextCursor = rows.Count > 0
            ? new MovieCursor(rows[^1].Id, DateTime.Now) // could use created_at from last row
            : cursor ?? new MovieCursor(0, DateTime.MinValue);

        return (rows.ToArray(), nextCursor);
    }

    public async Task<RatingSnapshot> GetRatingSnapshotAsync(int movieId)
    {
        const string sql = @"
            SELECT rating_count, rating_sum
            FROM media
            WHERE id = @id;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = movieId
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int count = reader.GetInt32(0);
            double sum = reader.GetDouble(1);
            double average = count > 0 ? sum / count : 0.0;
            return new RatingSnapshot(count, average);
        });

        return result.FirstOrDefault() ?? new RatingSnapshot(0, 0);
    }
}

public class SqlMovieCommandRepository(ISqlExecutor executor): Repository(executor), IMovieCommandRepository
{

    public async Task<Movie> Create(Movie movie)
    {

            // Insert media and get the inserted ID
            const string sqlInsertMedia = @"
                INSERT INTO media (title, description, type, rating_count, rating_sum)
                OUTPUT INSERTED.id, INSERTED.title, INSERTED.description
                VALUES (@title, @description, 'movie', 0, 0);
            ";

            var mediaParams = new Dictionary<string, object>
            {
                ["@title"] = movie.Title,
                ["@description"] = movie.Description
            };

            // Map the inserted media row to extract id
            var mediaRow = await _sqlExecutor.QueryAsync(sqlInsertMedia, mediaParams, reader =>
            {
                int id = reader.GetInt32(0);
                string title = reader.GetString(1);
                string description = reader.GetString(2);
                return new { Id = id, Title = title, Description = description };
            });

            if (mediaRow.Count == 0)
                throw new InvalidOperationException("Failed to insert media.");

            var mediaId = mediaRow[0].Id;

            // Insert into movie table
            const string sqlInsertMovie = @"
                INSERT INTO movie (media_id, genre_id, duration_minutes)
                VALUES (@mediaId, @genreId, @durationMinutes);
            ";

            var movieParams = new Dictionary<string, object>
            {
                ["@mediaId"] = mediaId,
                ["@genreId"] = movie.Genre.Id,
                ["@durationMinutes"] = (object?)movie.DurationMinutes ?? DBNull.Value
            };

            await _sqlExecutor.ExecuteNonQueryAsync(sqlInsertMovie, movieParams);

            // Hydrate movie object
            return Movie.Hydrate(mediaId, movie.Title, movie.Description, movie.Genre.Id, movie.Genre.Name, movie.DurationMinutes);

        }

}


using System.Data;
using System.Text;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;
using MoviDB.Domain.ValueObjects;
using MoviDB.Infrastructure.Database;

namespace MoviDB.Infrastructure.Repositories;

public class SqlMovieQueryRepository(ISqlExecutor executor) : Repository(executor), IMovieQueryRepository
{

    private Movie HydrateMovie(IDataRecord reader)
    {
        int id = reader.GetInt32(0);
        string title = reader.GetString(1);
        string description = reader.GetString(2);
        int genreId = reader.GetInt32(3);
        string genreName = reader.GetString(4);
        int durationMinutes = reader.GetInt32(8);

        return Movie.Hydrate(id, title, description, genreId, genreName, durationMinutes);
    }
    public async Task<Movie?> GetByIdAsync(int mediaId)
    {
        const string sql = "SELECT * FROM vw_movie WHERE media_id = @id";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = mediaId
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, HydrateMovie);

        return result.FirstOrDefault();
    }
    
    public async Task<Movie?> GetByTitleAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));

        const string sql = "SELECT * FROM vw_movie WHERE title = @title";

        var parameters = new Dictionary<string, object>
        {
            ["@title"] = title
        };

        var result = await _sqlExecutor.QueryAsync(sql, parameters, HydrateMovie);

        return result.FirstOrDefault();
    }

   public async Task<(MovieProjection[], MovieCursor)> GetNextBatchOfAllAsync(
        int batchSize,
        MovieCursor? cursor = null,
        MovieFilter? filter = null)
    {
        var (filterClause, parameters) = FilterMapper.BuildQueryClause(cursor?.CreatedAt, cursor?.Id, filter, MovieFilterMappings.Mappings);

        DateTime createdAt = new DateTime();
        var rows = await _sqlExecutor.QueryAsync($"SELECT TOP {batchSize} * FROM vw_movie {filterClause}", parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string description = reader.GetString(2);
            string genreName = reader.GetString(4);
            int ratingCount = reader.GetInt32(5);
            double ratingSum = reader.GetDouble(6);
            double average = ratingCount > 0 ? ratingSum / ratingCount : 0.0;
            var ratingSnapshot = new RatingSnapshot(ratingCount, average);
            int durationMinutes = reader.GetInt32(8);
            
            createdAt = reader.GetDateTime(9);
            return new MovieProjection(id, title, description, genreName, durationMinutes, ratingSnapshot);
        });

        var nextCursor = rows.Count > 0
            ? new MovieCursor(rows[^1].Id, createdAt)
            : cursor ?? new MovieCursor(0, DateTime.MinValue);

        return (rows.ToArray(), nextCursor);
    }


    public async Task<RatingSnapshot> GetRatingSnapshotAsync(int movieId)
    {
        const string sql = @"
            SELECT rating_count, rating_sum
            FROM media
            WHERE media_id = @id;
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
            OUTPUT INSERTED.id
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
            return new { Id = id };
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
    
    public async Task<Movie> Update(Movie movie)
    {
        if (movie == null)
            throw new ArgumentNullException(nameof(movie));

        // Update the media table (title, description)
        const string sqlUpdateMedia = @"
            UPDATE media
            SET title = @title,
                description = @description
            WHERE id = @mediaId;
        ";

        var mediaParams = new Dictionary<string, object>
        {
            ["@mediaId"] = movie.Id,
            ["@title"] = movie.Title,
            ["@description"] = movie.Description
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sqlUpdateMedia, mediaParams);

        // Update the movie table (genre_id, duration_minutes)
        const string sqlUpdateMovie = @"
            UPDATE movie
            SET genre_id = @genreId,
                duration_minutes = @durationMinutes
            WHERE media_id = @mediaId;
        ";

        var movieParams = new Dictionary<string, object>
        {
            ["@mediaId"] = movie.Id,
            ["@genreId"] = movie.Genre.Id,
            ["@durationMinutes"] = (object?)movie.DurationMinutes ?? DBNull.Value
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sqlUpdateMovie, movieParams);

        return Movie.Hydrate(movie.Id, movie.Title, movie.Description, movie.Genre.Id, movie.Genre.Name, movie.DurationMinutes);
    }
    
    /// <summary>
    /// Deletes a movie and its underlying media record by media ID.
    /// </summary>
    /// <param name="mediaId">The media ID of the movie to delete.</param>
    public async Task Delete(int mediaId)
    {
        if (mediaId <= 0)
            throw new ArgumentException("Media ID must be positive.", nameof(mediaId));

        const string sqlDeleteMovie = @"
            DELETE FROM movie
            WHERE media_id = @mediaId;
        ";

            const string sqlDeleteMedia = @"
            DELETE FROM media
            WHERE id = @mediaId;
        ";

        var parameters = new Dictionary<string, object>
        {
            ["@mediaId"] = mediaId
        };

        await _sqlExecutor.ExecuteNonQueryAsync(sqlDeleteMovie, parameters);
        await _sqlExecutor.ExecuteNonQueryAsync(sqlDeleteMedia, parameters);
    }
}

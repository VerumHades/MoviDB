using System.Text;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Repositories;
using MoviDB.Domain.ValueObjects;

namespace MoviDB.Infrastructure.Repositories;

public class SqlSeriesQueryRepository(ISqlExecutor sqlExecutor): Repository(sqlExecutor), ISeriesQueryRepository
{
    public async Task<Series?> GetByIdAsync(int mediaId)
    {
        const string sql = "SELECT * FROM vw_series WHERE media_id = @id";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = mediaId
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string description = reader.GetString(2);
            int genreId = reader.GetInt32(3);
            string genreName = reader.GetString(4);

            return Series.Hydrate(id, title, description, genreId, genreName);
        });

        return rows.FirstOrDefault();
    }

    public async Task<(SeriesProjection[], SeriesCursor)> GetNextBatchAsync(
        int batchSize,
        SeriesCursor? cursor = null,
        SeriesFilter? filter = null)
    {
        var sqlBuilder = new StringBuilder($"SELECT TOP {batchSize} * FROM vw_series");
        var parameters = new Dictionary<string, object>
        {
  
        };

        var conditions = new List<string>();

        // Cursor condition
        if (cursor != null)
        {
            conditions.Add("(CreatedAt > @cursorCreated OR (CreatedAt = @cursorCreated AND series_id > @cursorId))");
            parameters["@cursorCreated"] = cursor.CreatedAt;
            parameters["@cursorId"] = cursor.SeriesId;
        }

        // Filter mapping
        if (filter != null)
        {
            var filterConditions = new[]
            {
                (Value: (object?)filter.TitleContains, Column: "title", Operator: "LIKE", Transform: (Func<object, object>)(v => $"%{v}%")),
                (Value: filter.GenreId, Column: "genre_id", Operator: "=", Transform: null),
                (Value: filter.MinRating, Column: "rating", Operator: ">=", Transform: null),
                (Value: filter.MaxRating, Column: "rating", Operator: "<=", Transform: null)
            };

            foreach (var (value, column, sqlOperator, transform) in filterConditions)
            {
                if (value != null && !(value is string s && string.IsNullOrWhiteSpace(s)))
                {
                    string paramName = $"@{column}{parameters.Count}";
                    object finalValue = transform != null ? transform(value) : value;
                    conditions.Add($"{column} {sqlOperator} {paramName}");
                    parameters[paramName] = finalValue;
                }
            }
        }

        if (conditions.Count > 0)
            sqlBuilder.Append(" WHERE ").Append(string.Join(" AND ", conditions));

        sqlBuilder.Append(" ORDER BY CreatedAt, id");

        var sql = sqlBuilder.ToString();

        DateTime lastCreatedAt = DateTime.MinValue;
        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string description = reader.GetString(2);
            string genreName = reader.GetString(3);
            int seasonCount = reader.GetInt32(4);
            int ratingCount = reader.GetInt32(5);
            double ratingSum = reader.GetDouble(7);
            var ratingSnapshot = new RatingSnapshot(ratingCount, ratingSum);

            lastCreatedAt = reader.GetDateTime(6); // assuming CreatedAt is at index 6
            return new SeriesProjection(id, title, description, genreName, seasonCount, ratingSnapshot);
        });

        var nextCursor = rows.Count > 0
            ? new SeriesCursor(rows[^1].SeriesId, lastCreatedAt)
            : cursor ?? new SeriesCursor(0, DateTime.MinValue);

        return (rows.ToArray(), nextCursor);
    }


    public async Task<List<Season>> GetSeasonsAsync(int seriesId)
    {
        const string sql = "SELECT id, title, number FROM season WHERE series_id = @seriesId ORDER BY number";

        var parameters = new Dictionary<string, object>
        {
            ["@seriesId"] = seriesId
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            int number = reader.GetInt32(2);
            return Season.Hydrate(id, seriesId, number, title);
        });

        return rows;
    }
    

    public async Task<(SeriesEpisodeProjection[], SeriesEpisodeCursor)> GetNextBatchEpisodesAsync(
        int batchSize,
        SeriesEpisodeCursor? cursor = null,
        SeriesEpisodeFilter? filter = null)
    {
        var sqlBuilder = new StringBuilder($"SELECT TOP {batchSize} id, series_id, title, episode_number, CreatedAt FROM episode e");
        var parameters = new Dictionary<string, object>
        {

        };

        var conditions = new List<string>();

        // Cursor condition
        if (cursor != null)
        {
            conditions.Add("(e.CreatedAt > @cursorCreated OR (e.CreatedAt = @cursorCreated AND e.id > @cursorId))");
            parameters["@cursorCreated"] = cursor.CreatedAt;
            parameters["@cursorId"] = cursor.EpisodeId;
        }

        // Filter mapping
        if (filter != null)
        {
            var filterConditions = new[]
            {
                (Value: filter.SeriesId, Column: "e.series_id", Operator: "=", Transform: (Func<object, object>?)null),
                (Value: filter.SeasonId, Column: "e.season_id", Operator: "=", Transform: null)
            };

            foreach (var (value, column, sqlOperator, transform) in filterConditions)
            {
                if (value != null)
                {
                    string paramName = $"@{column.Replace(".", "_")}{parameters.Count}";
                    object finalValue = transform != null ? transform(value) : value;
                    conditions.Add($"{column} {sqlOperator} {paramName}");
                    parameters[paramName] = finalValue;
                }
            }
        }

        if (conditions.Count > 0)
            sqlBuilder.Append(" WHERE ").Append(string.Join(" AND ", conditions));

        sqlBuilder.Append(" ORDER BY e.CreatedAt, e.id");

        var sql = sqlBuilder.ToString();

        DateTime lastCreatedAt = DateTime.MinValue;
        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int id = reader.GetInt32(0);
            int seriesId = reader.GetInt32(1);
            string title = reader.GetString(2);
            int episodeNumber = reader.GetInt32(3);
            lastCreatedAt = reader.GetDateTime(4);

            return new SeriesEpisodeProjection(id, seriesId, title, episodeNumber);
        });

        var nextCursor = rows.Count > 0
            ? new SeriesEpisodeCursor(rows[^1].EpisodeId, lastCreatedAt)
            : cursor ?? new SeriesEpisodeCursor(0, DateTime.MinValue);

        return (rows.ToArray(), nextCursor);
    }
    public async Task<RatingSnapshot> GetRatingSnapshotAsync(int seriesId)
    {
        const string sql = "SELECT rating_count, rating_sum FROM media WHERE id = @id";

        var parameters = new Dictionary<string, object>
        {
            ["@id"] = seriesId
        };

        var rows = await _sqlExecutor.QueryAsync(sql, parameters, reader =>
        {
            int count = reader.GetInt32(0);
            double sum = reader.GetDouble(1);
            double average = count > 0 ? sum / count : 0.0;
            return new RatingSnapshot(count, average);
        });

        return rows.FirstOrDefault() ?? new RatingSnapshot(0, 0);
    }
}

public class SqlSeriesCommandRepository(ISqlExecutor sqlExecutor) : Repository(sqlExecutor), ISeriesCommandRepository
{
    public async Task<Series> Create(Series series)
    {
        // Insert into media and get inserted ID
        const string sqlInsertMedia = @"
            INSERT INTO media (title, description, type, rating_count, rating_sum)
            OUTPUT INSERTED.id, INSERTED.title, INSERTED.description
            VALUES (@title, @description, 'series', 0, 0);
        ";

        var mediaParams = new Dictionary<string, object>
        {
            ["@title"] = series.Title,
            ["@description"] = series.Description
        };

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

        const string sqlInsertSeries = @"
            INSERT INTO series (media_id, genre_id)
            VALUES (@mediaId, @genreId);
        ";

        var seriesParams = new Dictionary<string, object>
        {
            ["@mediaId"] = mediaId,
            ["@genreId"] = series.Genre.Id
        };

        await sqlExecutor.ExecuteNonQueryAsync(sqlInsertSeries, seriesParams);

        return Series.Hydrate(mediaId, series.Title, series.Description, series.Genre.Id, series.Genre.Name);
    }
    
    public async Task<Season> AddSeasonAsync(Season season)
    {
        const string sql = @"
        insert into season (series_id, title, number)
        output inserted.id
        values (@seriesId, @title, @number)";

        var parameters = new Dictionary<string, object>
        {
            ["@seriesId"] = season.SeriesId,
            ["@title"] = season.Title,
            ["@number"] = season.Number
        };

        // ExecuteScalarAsync to get the generated ID
        var insertedId = await _sqlExecutor.ExecuteScalarAsync<int>(sql, parameters);
        season.Id = insertedId;

        return season;
    }

    public async Task<Episode> AddEpisodeAsync(Episode episode)
    {
        const string sql = @"
        insert into episode (season_id, title, episode_number)
        output inserted.id
        values (@seasonId, @title, @episodeNumber)";

        var parameters = new Dictionary<string, object>
        {
            ["@seasonId"] = episode.SeasonId,
            ["@title"] = episode.Title,
            ["@episodeNumber"] = episode.EpisodeNumber
        };

        // ExecuteScalarAsync to get the generated ID
        var insertedId = await _sqlExecutor.ExecuteScalarAsync<int>(sql, parameters);
        episode.Id = insertedId;

        return episode;
    }


}
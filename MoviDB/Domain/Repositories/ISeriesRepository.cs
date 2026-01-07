using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.ValueObjects;

namespace MoviDB.Domain.Repositories;

public interface ISeriesQueryRepository
{
    /// <summary>
    /// Returns the Series aggregate with its underlying Media.
    /// Episodes are not loaded in memory.
    /// </summary>
    Task<Series?> GetByIdAsync(int mediaId);
    Task<(SeriesProjection[],SeriesCursor)> GetNextBatchAsync(int batchSize, SeriesCursor? cursor = null, SeriesFilter? filter = null);
    Task<List<Season>> GetSeasonsAsync(int seriesId);
    Task<(SeriesEpisodeProjection[],SeriesEpisodeCursor)> GetNextBatchEpisodesAsync(int batchSize, SeriesEpisodeCursor? cursor = null, SeriesEpisodeFilter? filter = null);
    Task<RatingSnapshot> GetRatingSnapshotAsync(int seriesId);
}

public interface ISeriesCommandRepository
{
    /// <summary>
    /// Creates a new Series along with the underlying Media row.
    /// </summary>
    Task<Series> Create(Series series);
    /// <summary>
    /// Adds a season to the series.
    /// </summary>
    Task<Season> AddSeasonAsync(Season season);
    /// <summary>
    /// Adds an episode to a season. Episodes are stored independently.
    /// </summary>
    Task<Episode> AddEpisodeAsync(Episode episode);
}
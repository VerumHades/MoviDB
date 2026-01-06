using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.ValueObjects;
using MoviDB.Domain.Views;

namespace MoviDB.Domain.Repositories;

public interface ISeriesRepository
{
    /// <summary>
    /// Returns the Series aggregate with its underlying Media.
    /// Episodes are not loaded in memory.
    /// </summary>
    Task<Series?> GetByIdAsync(int mediaId);

    /// <summary>
    /// Creates a new Series along with the underlying Media row.
    /// </summary>
    Task<Series> Create(Series series);
    
    Task<(SeriesView[],SeriesCursor)> GetNextBatchAsync(int batchSize, SeriesCursor? cursor = null, SeriesFilter? filter = null);
    Task<List<Season>> GetSeasonsAsync(int seriesId);
    Task<(SeriesEpisodeView[],SeriesEpisodeCursor)> GetNextBatchEpisodesAsync(int batchSize, SeriesEpisodeCursor? cursor = null, SeriesEpisodeFilter? filter = null);
    
    /// <summary>
    /// Adds a season to the series.
    /// </summary>
    Task AddSeasonAsync(Season season);
    /// <summary>
    /// Adds an episode to a season. Episodes are stored independently.
    /// </summary>
    Task AddEpisodeAsync(Episode episode);
    
    /// <summary>
    /// Adds multiple seasons to a series in a single operation.
    /// </summary>
    Task AddSeasonsBulkAsync(IEnumerable<Season> seasons);

    /// <summary>
    /// Adds multiple episodes to their respective seasons in a single operation.
    /// </summary>
    Task AddEpisodesBulkAsync(IEnumerable<Episode> episodes);
    
    Task<RatingSnapshot> GetRatingSnapshotAsync(int seriesId);
}
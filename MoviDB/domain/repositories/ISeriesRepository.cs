using MoviDB.domain.entities;

namespace MoviDB.domain.repositories;

public interface ISeriesRepository
{
    /// <summary>
    /// Returns the Series aggregate with its underlying Media.
    /// Episodes are not loaded in memory.
    /// </summary>
    Series GetById(int mediaId);

    /// <summary>
    /// Creates a new Series along with the underlying Media row.
    /// </summary>
    Series Create(string title, string description, Genre genre, int userId);

    /// <summary>
    /// Adds a season to the series.
    /// </summary>
    void AddSeason(Season season);

    /// <summary>
    /// Adds an episode to a season. Episodes are stored independently.
    /// </summary>
    void AddEpisode(Episode episode);

    /// <summary>
    /// Fetches episodes of a season, cursor-paged.
    /// </summary>
    IReadOnlyList<Episode> GetEpisodes(int seasonId, int pageSize, int? cursorAfterId = null);
}
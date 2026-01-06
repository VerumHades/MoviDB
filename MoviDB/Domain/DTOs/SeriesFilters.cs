namespace MoviDB.Domain.DTOs;

/// <summary>
/// Filtering options for querying series.
/// </summary>
public record SeriesFilter(
    string? TitleContains = null,
    int? GenreId = null,
    double? MinRating = null,
    double? MaxRating = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null
);

/// <summary>
/// Filtering options for querying episodes.
/// </summary>
public record SeriesEpisodeFilter(
    string? TitleContains = null,
    int? SeasonNumber = null,
    int? EpisodeNumber = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null
);
using MoviDB.Domain.ValueObjects;

namespace MoviDB.Domain.DTOs;

/// <summary>
/// Lightweight read-only projection of a series for listing.
/// </summary>
public record SeriesProjection(
    int SeriesId,
    string Title,
    string Description,
    string GenreName,
    int SeasonCount,
    RatingSnapshot Rating
);

/// <summary>
/// Lightweight read-only projection of a season for listing.
/// </summary>
public record SeasonProjection(
    int SeasonId,
    int SeriesId,
    string Title,
    int Number
);

/// <summary>
/// Lightweight read-only projection of an episode for listing.
/// </summary>
public record SeriesEpisodeProjection(
    int EpisodeId,
    int SeasonId,
    string Title,
    int EpisodeNumber
);
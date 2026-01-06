using MoviDB.Domain.ValueObjects;

namespace MoviDB.Domain.Views;

/// <summary>
/// Lightweight read-only projection of a series for listing.
/// </summary>
public record SeriesView(
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
public record SeasonView(
    int SeasonId,
    int SeriesId,
    string Title,
    int Number
);

/// <summary>
/// Lightweight read-only projection of an episode for listing.
/// </summary>
public record SeriesEpisodeView(
    int EpisodeId,
    int SeasonId,
    string Title,
    int EpisodeNumber,
    DateTime CreatedAt
);
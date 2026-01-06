namespace MoviDB.Application.DTOs;

/// <summary>
/// DTO for creating an episode in-memory.
/// </summary>
public record EpisodeCreationData(
    string Title,
    int EpisodeNumber
);

/// <summary>
/// DTO for creating a season with its episodes in-memory.
/// </summary>
public record SeasonCreationData(
    string Title,
    int Number,
    List<EpisodeCreationData> Episodes
);

/// <summary>
/// DTO for creating a series with seasons and episodes in-memory.
/// </summary>
public record SeriesCreationData(
    string Title,
    string Description,
    string GenreName,
    List<SeasonCreationData> Seasons
);

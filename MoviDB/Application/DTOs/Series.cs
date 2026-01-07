using System.Text.Json.Serialization;

namespace MoviDB.Application.DTOs;

/// <summary>
/// DTO for creating an episode in-memory.
/// </summary>
public record EpisodeCreationData(
    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("episode_number")]
    int EpisodeNumber
);

/// <summary>
/// DTO for creating a season with its episodes in-memory.
/// </summary>
public record SeasonCreationData(
    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("number")]
    int Number,

    [property: JsonPropertyName("episodes")]
    List<EpisodeCreationData> Episodes
);

/// <summary>
/// DTO for creating a series with seasons and episodes in-memory.
/// </summary>
public record SeriesCreationData(
    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("description")]
    string Description,

    [property: JsonPropertyName("genre_name")]
    string GenreName,

    [property: JsonPropertyName("seasons")]
    List<SeasonCreationData> Seasons
);
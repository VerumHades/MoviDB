namespace MoviDB.Domain.DTOs;

public record MovieCursor(int Id, DateTime CreatedAt);

/// <summary>
/// Cursor for paging through Series.
/// </summary>
public record SeriesCursor(int SeriesId, DateTime CreatedAt);

/// <summary>
/// Cursor for paging through episodes of a series.
/// </summary>
public record SeriesEpisodeCursor(int EpisodeId, DateTime CreatedAt);
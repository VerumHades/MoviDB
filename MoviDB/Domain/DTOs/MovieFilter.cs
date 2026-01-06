namespace MoviDB.Domain.DTOs;

public record MovieFilter(
    string? TitleContains = null,
    int? GenreId = null,
    double? MinRating = null,
    double? MaxRating = null,
    int? MinDuration = null,
    int? MaxDuration = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null
);
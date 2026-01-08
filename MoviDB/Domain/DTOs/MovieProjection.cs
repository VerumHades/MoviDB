using MoviDB.Domain.ValueObjects;

namespace MoviDB.Domain.DTOs;

public record MovieProjection(
    int Id,
    string Title,
    string Description,
    string GenreName,
    int DurationMinutes,
    RatingSnapshot RatingSnapshot
);
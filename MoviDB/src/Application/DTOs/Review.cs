namespace MoviDB.Application.DTOs;

public record ReviewCreationData(
    int MediaId,
    int UserId,
    string Title,
    string Content,
    double Rating
);

public sealed record ReviewUpdateData(
    int MediaId,
    int UserId,
    string Title,
    string Content,
    double Rating
);
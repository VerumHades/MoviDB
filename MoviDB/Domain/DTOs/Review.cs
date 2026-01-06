namespace MoviDB.Domain.DTOs;

public record ReviewCreationData(
    int MediaId,
    int UserId,
    string Title,
    string Description,
    double Rating
);

public record ReviewUpdateData(int MediaId, int UserId, string Title, string Description, double Rating) : ReviewCreationData(MediaId, UserId, Title, Description, Rating);
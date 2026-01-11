namespace MoviDB.Application.DTOs;

/// <summary>
/// Data required to create a review using media title and username.
/// </summary>
public record ReviewCreationData(
    string MediaTitle,
    string Username,
    string Title,
    string Content,
    double Rating
);

/// <summary>
/// Data required to update a review using media title and username.
/// </summary>
public sealed record ReviewUpdateData(
    string MediaTitle,
    string Username,
    string Title,
    string Content,
    double Rating
);
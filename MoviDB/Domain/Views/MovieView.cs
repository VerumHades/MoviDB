using MoviDB.Domain.ValueObjects;

namespace MoviDB.Domain.Views;

public record MovieView(
    int Id,
    string Title,
    string Description,
    string GenreName,
    RatingSnapshot RatingSnapshot
);
using MoviDB.domain.entities;

namespace MoviDB.domain.repositories;

public interface IReviewRepository
{
    Review GetById(int reviewId);

    /// <summary>
    /// Creates a review for a given media.
    /// </summary>
    Review Create(int mediaId, int userId, string title, string content, double rating);

    /// <summary>
    /// Fetches reviews for a media, cursor-paged.
    /// </summary>
    IReadOnlyList<Review> GetByMediaId(int mediaId, int pageSize, int? cursorAfterId = null);
}
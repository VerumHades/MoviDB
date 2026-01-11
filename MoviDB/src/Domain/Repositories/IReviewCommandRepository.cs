using MoviDB.Domain.Entities.Media;

namespace MoviDB.Domain.Repositories;

public interface IReviewCommandRepository
{
    /// <summary>
    /// Persists a review
    /// </summary>
    /// <param name="review">The review to persist</param>
    /// <returns>Updated review with an id</returns>
    Task CreateAsync(Review review);
    
    /// <summary>
    /// Persists a review
    /// </summary>
    /// <param name="review">The review to persist</param>
    /// <returns>Updated review with an id</returns>
    Task UpdateAsync(Review review);
    
    /// <summary>
    /// Removes a review by id.
    /// </summary>
    Task RemoveAsync(int mediaId, int userId);
}
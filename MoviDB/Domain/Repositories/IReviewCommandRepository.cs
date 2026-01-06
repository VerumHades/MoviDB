using MoviDB.Domain.DTOs;

namespace MoviDB.Domain.Repositories;

public interface IReviewCommandRepository
{
    /// <summary>
    /// Persists a review
    /// </summary>
    /// <param name="review">The review to persist</param>
    /// <returns>Updated review with an id</returns>
    Task AddReviewAsync(ReviewCreationData review);
    
    /// <summary>
    /// Persists a review
    /// </summary>
    /// <param name="review">The review to persist</param>
    /// <returns>Updated review with an id</returns>
    Task UpdateReviewAsync(ReviewUpdateData review);
    
    /// <summary>
    /// Removes a review by id.
    /// </summary>
    Task RemoveReviewAsync(int reviewId, int userId);
}
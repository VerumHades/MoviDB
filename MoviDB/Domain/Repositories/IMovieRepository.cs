using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.ValueObjects;
using MoviDB.Domain.Views;

namespace MoviDB.Domain.Repositories;

public interface IMovieRepository
{
    /// <summary>
    /// Returns the Movie aggregate with its underlying Media.
    /// </summary>
    Task<Movie?> GetByIdAsync(int mediaId);
    
    Task<(MovieView[],MovieCursor)> GetNextBatchOfAllAsync(int batchSize, MovieCursor? cursor = null, MovieFilter? filter = null);
    
    Task<RatingSnapshot> GetRatingSnapshotAsync(int movieId);
    
    /// <summary>
    /// Creates a new Movie along with the underlying Media row.
    /// </summary>
    Task<Movie> Create(Movie movie);
}
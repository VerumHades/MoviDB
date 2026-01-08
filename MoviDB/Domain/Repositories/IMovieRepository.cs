using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.ValueObjects;

namespace MoviDB.Domain.Repositories;

public interface IMovieQueryRepository
{
    /// <summary>
    /// Returns the Movie aggregate with its underlying Media.
    /// </summary>
    Task<Movie?> GetByIdAsync(int mediaId);
    
    Task<Movie?> GetByTitleAsync(string title);
    
    Task<(MovieProjection[],MovieCursor)> GetNextBatchOfAllAsync(int batchSize, MovieCursor? cursor = null, MovieFilter? filter = null);
    
    Task<RatingSnapshot> GetRatingSnapshotAsync(int movieId);
    
}

public interface IMovieCommandRepository
{
    /// <summary>
    /// Creates a new Movie along with the underlying Media row.
    /// </summary>
    Task<Movie> Create(Movie movie);
    
    Task<Movie> Update(Movie movie);
    
    Task Delete(int mediaId);
}
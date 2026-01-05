using MoviDB.domain.entities;

namespace MoviDB.domain.repositories;

public interface IMovieRepository
{
    /// <summary>
    /// Returns the Movie aggregate with its underlying Media.
    /// </summary>
    Movie GetById(int mediaId);

    /// <summary>
    /// Creates a new Movie along with the underlying Media row.
    /// </summary>
    Movie Create(string title, string description, Genre genre, int durationMinutes, int userId);
}
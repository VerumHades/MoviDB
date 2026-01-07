namespace MoviDB.Domain.Entities.Media;

public sealed class Movie : Media
{
    public Genre Genre { get; }
    public int DurationMinutes { get; }

    public Movie(string title, string description, Genre genre, int durationMinutes) : base(title, description)
    {
        Genre = genre;
        DurationMinutes = durationMinutes;
    }
    
    /// <summary>
    /// Constructs a Movie instance from database values.
    /// </summary>
    public static Movie Hydrate(
        int mediaId,
        string title,
        string description,
        int genreId,
        string genreName,
        int durationMinutes)
    {
        var genre = Genre.Hydrate(genreId, genreName);
        var movie = new Movie(title, description, genre, durationMinutes);

        // Assign ID (assuming Media has a setter or internal constructor)
        movie.Id = mediaId; // You may need to add this method in Media base class

        return movie;
    }
}

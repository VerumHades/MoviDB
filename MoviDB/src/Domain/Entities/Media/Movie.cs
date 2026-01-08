namespace MoviDB.Domain.Entities.Media;

public sealed class Movie : Media
{
    public Genre Genre
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(value), "Genre cannot be null");
    }

    public int DurationMinutes
    {
        get;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Duration in minutes must be positive", nameof(value));
            field = value;
        }
    }
    
    public Movie(string title, string description, Genre genre, int durationMinutes)
        : base(title, description)
    {
        Genre = genre;               // uses guarded setter
        DurationMinutes = durationMinutes; // uses guarded setter
    }

    /// <summary>
    /// Hydrates a Movie from database values.
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
        var movie = new Movie(title, description, genre, durationMinutes)
        {
            Id = mediaId
        };

        return movie;
    }
}
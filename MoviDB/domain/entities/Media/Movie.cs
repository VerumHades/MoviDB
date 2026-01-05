namespace MoviDB.domain.entities;

public sealed class Movie
{
    public Media Media { get; }
    public Genre Genre { get; }
    public int DurationMinutes { get; }

    public Movie(Media media, Genre genre, int durationMinutes)
    {
        Media = media ?? throw new ArgumentNullException(nameof(media));
        if (media.Type != MediaType.Movie)
            throw new ArgumentException("Media must be of type Movie");
        Genre = genre ?? throw new ArgumentNullException(nameof(genre));
        DurationMinutes = durationMinutes;
    }
}

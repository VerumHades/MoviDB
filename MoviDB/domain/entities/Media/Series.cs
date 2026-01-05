namespace MoviDB.domain.entities;

public sealed class Series
{
    /// <summary>
    /// The underlying Media aggregate root that contains identity, ratings, title, description, etc.
    /// </summary>
    public Media Media { get; }

    /// <summary>
    /// The genre of the series.
    /// </summary>
    public Genre Genre { get; }

    /// <summary>
    /// Creates a new Series domain entity wrapping a Media aggregate.
    /// </summary>
    /// <param name="media">The Media aggregate. Must have MediaType.Series.</param>
    /// <param name="genre">The genre of the series.</param>
    public Series(Media media, Genre genre)
    {
        Media = media ?? throw new ArgumentNullException(nameof(media));
        if (media.Type != MediaType.Series)
            throw new ArgumentException("Media must be of type Series", nameof(media));

        Genre = genre ?? throw new ArgumentNullException(nameof(genre));
    }
}
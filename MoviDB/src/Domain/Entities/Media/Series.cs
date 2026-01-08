using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Series: Media
{
    public Genre Genre { get; }
    
    public Series(string title, string description, Genre genre): base(title, description)
    {
        Genre = genre;
    }
    
    /// <summary>
    /// Constructs a Series instance from database values.
    /// </summary>
    public static Series Hydrate(
        int mediaId,
        string title,
        string description,
        int genreId,
        string genreName)
    {
        var genre = Genre.Hydrate(genreId, genreName);
        var series = new Series(title, description, genre);

        // Assign ID (assuming Media has a setter or internal constructor)
        series.Id = mediaId; // You may need to add this method in Media base class

        return series;
    }
}
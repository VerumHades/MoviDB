using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public enum MediaType
{
    Movie,
    Series
}

public sealed class Media: TimestampedEntity
{
    public string Title { get; }
    public string Description { get; }
    public MediaType Type { get; }

    private Media(string title, string description, MediaType type)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description cannot be empty");
        
        Title = title;
        Description = description;
        Type = type;
    }
    
    /// <summary>
    /// Factory method for creating a Movie media.
    /// </summary>
    public static Movie CreateMovie(string title, string description, Genre genre, int durationMinutes)
    {
        return new Movie(
            new Media(title, description, MediaType.Movie),
            genre, 
            durationMinutes
        );
    }

    /// <summary>
    /// Factory method for creating a Series media.
    /// </summary>
    public static Series CreateSeries(string title, string description, Genre genre)
    {
        return new Series(
            new Media(title, description, MediaType.Series),
            genre
        );
    }
}
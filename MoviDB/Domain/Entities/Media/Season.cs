using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Season: Entity
{
    public int SeriesId { get; }
    public string Title { get; }
    public int Number { get; }
    
    private const int MaxTitleLength = 255;
    
    public Season(int seriesId, int number, string title)
    {
        if (number <= 0) throw new ArgumentException("Season number must be positive");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Season title cannot be empty.", nameof(title));
        if (title.Length > MaxTitleLength)
            throw new ArgumentException($"Season title cannot exceed {MaxTitleLength} characters.", nameof(title));
        
        SeriesId = seriesId;
        Number = number;
        Title = title;
    }

    public static Season Hydrate(int id, int seriesId, int number, string title)
    {
        var season = new Season(seriesId, number, title)
        {
            Id = id
        };
        return season;
    }
}
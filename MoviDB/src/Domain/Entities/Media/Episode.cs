using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Episode: Entity
{
    public int SeasonId { get; }
    public string Title { get; }
    public int EpisodeNumber { get; }
    
    private const int MaxTitleLength = 255;
    public Episode(int seasonId, string title, int episodeNumber)
    {
        if (episodeNumber <= 0) throw new ArgumentException("Episode number must be positive");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (title.Length > MaxTitleLength)
            throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters.", nameof(title));

        
        SeasonId = seasonId;
        Title = title;
        EpisodeNumber = episodeNumber;
    }
}
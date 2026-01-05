namespace MoviDB.domain.entities;

public sealed class Episode
{
    public int Id { get; }
    public int SeasonId { get; }
    public string Title { get; }
    public int EpisodeNumber { get; }
    public DateTime CreatedAt { get; }

    public Episode(int id, int seasonId, string title, int episodeNumber, DateTime createdAt)
    {
        if (episodeNumber <= 0) throw new ArgumentException("Episode number must be positive");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Episode title cannot be empty");

        Id = id;
        SeasonId = seasonId;
        Title = title;
        EpisodeNumber = episodeNumber;
        CreatedAt = createdAt;
    }
}
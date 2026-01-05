namespace MoviDB.domain.entities;

public enum MediaType
{
    Movie,
    Series
}

public sealed class Media
{
    public int Id { get; }
    public int UserId { get; }
    public DateTime CreatedAt { get; }
    public string Title { get; }
    public string Description { get; }
    public MediaType Type { get; }
    public int RatingCount { get; private set; }
    public double RatingSum { get; private set; }
    public double Rating => RatingCount == 0 ? 0 : RatingSum / RatingCount;

    public Media(int id, int userId, DateTime createdAt, string title, string description, MediaType type)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description cannot be empty");

        Id = id;
        UserId = userId;
        CreatedAt = createdAt;
        Title = title;
        Description = description;
        Type = type;
        RatingCount = 0;
        RatingSum = 0;
    }
}
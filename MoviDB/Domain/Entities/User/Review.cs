using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Review
{
    public int MediaId { get; }
    public int UserId { get; }
    public string Title { get; }
    public string Content { get; }
    public double Rating { get; }

    public Review(int mediaId, int userId, string title, string content, double rating)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Review title cannot be empty");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Review content cannot be empty");
        if (rating < 0 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be 0-5");
        
        MediaId = mediaId;
        UserId = userId;
        Title = title;
        Content = content;
        Rating = rating;
    }
}
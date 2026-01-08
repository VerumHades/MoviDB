using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Review
{
    public int MediaId { get; }
    public int UserId { get; }
    public string Title { get; }
    public string Content { get; }
    public double Rating { get; }
    
    private const int MaxTitleLength = 255;
    private const int MaxDescriptionLength = 300;


    public Review(int mediaId, int userId, string title, string content, double rating)
    {
        if (rating < 0 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be 0-5");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (title.Length > MaxTitleLength)
            throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters.", nameof(title));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Description cannot be empty.", nameof(content));
        if (content.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description cannot exceed {MaxDescriptionLength} characters.", nameof(content));

        
        MediaId = mediaId;
        UserId = userId;
        Title = title;
        Content = content;
        Rating = rating;
    }
}
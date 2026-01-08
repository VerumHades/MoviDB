using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public abstract class Media : Entity
{
    private const int MaxTitleLength = 255;
    private const int MaxDescriptionLength = 300;

    public string Title
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty.", nameof(value));
            if (value.Length > MaxTitleLength)
                throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters.", nameof(value));
            field = value;
        }
    }

    public string Description
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be empty.", nameof(value));
            if (value.Length > MaxDescriptionLength)
                throw new ArgumentException($"Description cannot exceed {MaxDescriptionLength} characters.",
                    nameof(value));
            field = value;
        }
    }

    protected Media(string title, string description)
    {
        Title = title;           // invokes guarded setter
        Description = description; // invokes guarded setter
    }
}
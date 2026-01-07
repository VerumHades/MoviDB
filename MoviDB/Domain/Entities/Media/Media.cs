using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public enum MediaType
{
    Movie,
    Series
}

public abstract class Media: Entity
{
    public string Title { get; }
    public string Description { get; }

    protected Media(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description cannot be empty");
        
        Title = title;
        Description = description;
    }
}
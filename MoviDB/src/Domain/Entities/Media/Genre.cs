using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Genre: Entity
{
    public string Name { private set; get; }
    
    private const int MaxNameLength = 255;
    private Genre(int id, string name): base(id)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Genre name cannot be empty");
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Name cannot exceed {MaxNameLength} characters.", nameof(name));

        Name = name;
    }
    
    /// <summary>
    /// Used by repositories to rehydrate the aggregate from persistence.
    /// Do NOT use this method in application code.
    /// </summary>
    internal static Genre Hydrate(int id, string name)
    {
        var genre = new Genre(id, name);
        return genre;
    }
}
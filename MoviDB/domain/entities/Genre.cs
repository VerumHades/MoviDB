namespace MoviDB.domain.entities;

public sealed class Genre
{
    public int Id { get; }
    public string Name { get; }

    public Genre(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Genre name cannot be empty");
        Id = id;
        Name = name;
    }
}
namespace MoviDB.Domain.Exceptions;

public class GenreNotFoundException : Exception
{
    public string GenreName { get; }

    public GenreNotFoundException(string name)
        : base($"Genre with name {name} was not found.")
    {
        GenreName = name;
    }
}
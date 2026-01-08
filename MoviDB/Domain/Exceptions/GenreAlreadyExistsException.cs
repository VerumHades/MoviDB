namespace MoviDB.Domain.Exceptions;

public class GenreAlreadyExistsException: Exception
{
    public string GenreName { get; }

    public GenreAlreadyExistsException(string name)
        : base($"Genre {name} already exists.")
    {
        GenreName = name;
    }
}
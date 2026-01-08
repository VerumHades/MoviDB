
namespace MoviDB.Domain.Exceptions;

public class MovieNotFoundException : Exception
{
    public MovieNotFoundException()
    {
    }

    public MovieNotFoundException(string movieTitle)
        : base($"Movie with title '{movieTitle}' not found.")
    {
    }

    public MovieNotFoundException(string movieTitle, Exception inner)
        : base($"Movie with title '{movieTitle}' not found.", inner)
    {
    }
}

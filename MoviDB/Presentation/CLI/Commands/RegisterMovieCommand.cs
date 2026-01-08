using MoviDB.Application.Services;
using MoviDB.Domain.Entities.Media;
using MoviDB.Presentation.CLI.Constraints;

namespace MoviDB.Presentation.CLI.Commands;

public class RegisterMovieCommand : ICommand
{
    private readonly MovieService _movieService;

    public RegisterMovieCommand(MovieService movieService)
    {
        _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
    }

    public string Name => "RegisterMovie";
    public string Description => "Registers a new movie with title, description, genre, and duration.";

    public List<CommandParameter> GetParameters()
    {
        return new List<CommandParameter>
        {
            new ("title", "Title of the movie", typeof(string), false, [new StringLengthConstraint(1,255)]),
            new ("description", "Description of the movie", typeof(string), false, [new StringLengthConstraint(1,300)]),
            new ("genre", "Genre of the movie", typeof(string), false, [new StringLengthConstraint(1,255)]),
            new ("durationMinutes", "Duration of the movie in minutes", typeof(int), false)
        };
    }

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        if (!parameterValues.TryGetValue("title", out var titleObj) || titleObj is not string title)
            throw new ArgumentException("Parameter 'title' is required and must be a string.");

        if (!parameterValues.TryGetValue("description", out var descriptionObj) || descriptionObj is not string description)
            throw new ArgumentException("Parameter 'description' is required and must be a string.");

        if (!parameterValues.TryGetValue("genre", out var genreObj) || genreObj is not string genre)
            throw new ArgumentException("Parameter 'genre' is required and must be a string.");

        if (!parameterValues.TryGetValue("durationMinutes", out var durationObj) || durationObj is not int durationMinutes)
            throw new ArgumentException("Parameter 'durationMinutes' is required and must be an integer.");

        RegisterMovieAsync(title, description, genre, durationMinutes).GetAwaiter().GetResult();
    }

    private async Task RegisterMovieAsync(string title, string description, string genre, int durationMinutes)
    {
        try
        {
            Movie movie = await _movieService.RegisterMovieAsync(title, description, genre, durationMinutes);
            Console.WriteLine($"Successfully registered movie: {movie.Title} (ID: {movie.Id})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to register movie: {ex.Message}");
        }
    }
}
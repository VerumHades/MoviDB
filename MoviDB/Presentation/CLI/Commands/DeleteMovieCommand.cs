using MoviDB.Application.Services;
using MoviDB.Presentation.CLI.Constraints;

namespace MoviDB.Presentation.CLI.Commands;

public class DeleteMovieCommand : ICommand
{
    private readonly MovieService _movieService;

    public DeleteMovieCommand(MovieService movieService)
    {
        _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
    }

    public string Name => "DeleteMovie";

    public string Description => "Deletes an existing movie by title.";

    public List<CommandParameter> GetParameters()
    {
        return new List<CommandParameter>
        {
            new CommandParameter(
                "title",
                "Title of the movie to delete",
                typeof(string),
                IsOptional: false,
                Constraints: new IParameterConstraint[]
                {
                    new StringLengthConstraint(1, 255)
                }
            )
        };
    }

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        if (!parameterValues.TryGetValue("title", out var titleObj) || titleObj is not string title)
            throw new ArgumentException("Parameter 'title' is required and must be a string.");

        DeleteMovieAsync(title, output).GetAwaiter().GetResult();
    }

    private async Task DeleteMovieAsync(string title, TextWriter output)
    {
        try
        {
            await _movieService.DeleteMovieByTitleAsync(title);
            await output.WriteLineAsync($"Successfully deleted movie: {title}");
        }
        catch (Exception ex)
        {
            await output.WriteLineAsync($"Failed to delete movie: {ex.Message}");
        }
    }
}
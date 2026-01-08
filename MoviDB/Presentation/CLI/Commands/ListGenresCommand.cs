using MoviDB.Application.Services;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Repositories;

namespace MoviDB.Presentation.CLI.Commands;

public class ListGenresCommand : ICommand
{
    private readonly GenreService _genreService;

    public ListGenresCommand(GenreService genreService)
    {
        _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
    }

    public string Name => "ListGenres";
    public string Description => "Lists all genres.";

    public List<CommandParameter> GetParameters() => new List<CommandParameter>();

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        ListGenresAsync(output).GetAwaiter().GetResult();
    }

    private async Task ListGenresAsync(TextWriter output)
    {
        try
        {
            var genres = await _genreService.ListGenresAsync();
            if (genres.Count == 0)
            {
                output.WriteLine("No genres found.");
                return;
            }

            output.WriteLine("Genres:");
            foreach (var genre in genres)
                output.WriteLine($"- {genre.Name} (ID: {genre.Id})");
        }
        catch (Exception ex)
        {
            output.WriteLine($"Failed to list genres: {ex.Message}");
        }
    }
}
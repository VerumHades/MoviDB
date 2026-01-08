
using MoviDB.Application.Services;

namespace MoviDB.Presentation.CLI.Commands;

public class CreateGenreCommand : ICommand
{
    private readonly GenreService _genreService;

    public CreateGenreCommand(GenreService genreService)
    {
        _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
    }

    public string Name => "CreateGenre";
    public string Description => "Creates a new genre.";

    public List<CommandParameter> GetParameters() =>
        new List<CommandParameter> { new("name", "Name of the genre", typeof(string), false) };

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        if (!parameterValues.TryGetValue("name", out var nameObj) || nameObj is not string name)
            throw new ArgumentException("Parameter 'name' is required and must be a string.");

        AddGenreAsync(name, output).GetAwaiter().GetResult();
    }

    private async Task AddGenreAsync(string name, TextWriter output)
    {
        try
        {
            var genre = await _genreService.AddGenreAsync(name);
            output.WriteLine($"Successfully created genre: {genre.Name} (ID: {genre.Id})");
        }
        catch (Exception ex)
        {
            output.WriteLine($"Failed to create genre: {ex.Message}");
        }
    }
}


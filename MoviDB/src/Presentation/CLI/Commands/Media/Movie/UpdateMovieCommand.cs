using MoviDB.Application.Services;
using MoviDB.Domain.Entities.Media;
using MoviDB.Presentation.CLI.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MoviDB.Application.DTOs;

namespace MoviDB.Presentation.CLI.Commands;

public class UpdateMovieCommand : ICommand
{
    private readonly MovieService _movieService;

    public UpdateMovieCommand(MovieService movieService)
    {
        _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
    }

    public string Name => "UpdateMovie";

    public string Description => "Updates an existing movie. All fields are optional; provide only the fields to change.";

    public List<CommandParameter> GetParameters()
    {
        return new List<CommandParameter>
        {
            new CommandParameter(
                "title",
                "Current title of the movie to update",
                typeof(string),
                IsOptional: false,
                Constraints: new IParameterConstraint[] { new StringLengthConstraint(1, 255) }
            ),
            new CommandParameter(
                "newTitle",
                "New title of the movie",
                typeof(string),
                IsOptional: true,
                Constraints: new IParameterConstraint[] { new StringLengthConstraint(1, 255) }
            ),
            new CommandParameter(
                "description",
                "New description of the movie",
                typeof(string),
                IsOptional: true,
                Constraints: new IParameterConstraint[] { new StringLengthConstraint(1, 300) }
            ),
            new CommandParameter(
                "genre",
                "New genre name of the movie",
                typeof(string),
                IsOptional: true,
                Constraints: new IParameterConstraint[] { new StringLengthConstraint(1, 255) }
            ),
            new CommandParameter(
                "durationMinutes",
                "New duration in minutes",
                typeof(int),
                IsOptional: true,
                Constraints: Array.Empty<IParameterConstraint>()
            )
        };
    }

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        if (!parameterValues.TryGetValue("title", out var titleObj) || titleObj is not string title)
        {
            throw new ArgumentException("Parameter 'title' is required and must be a string.");
        }

        string? newTitle = parameterValues.TryGetValue("newTitle", out var newTitleObj) && newTitleObj is string nt ? nt : null;
        string? newDescription = parameterValues.TryGetValue("description", out var descObj) && descObj is string d ? d : null;
        string? newGenre = parameterValues.TryGetValue("genre", out var genreObj) && genreObj is string g ? g : null;

        int? duration = null;
        if (parameterValues.TryGetValue("durationMinutes", out var durObj))
        {
            try
            {
                duration = Convert.ToInt32(durObj);
            }
            catch
            {
                throw new ArgumentException("Parameter 'durationMinutes' must be an integer.");
            }
        }

        UpdateMovieAsync(title, newTitle, newDescription, newGenre, duration, output).GetAwaiter().GetResult();
    }

    private async Task UpdateMovieAsync(string currentTitle, string? newTitle, string? newDescription, string? newGenre, int? duration, TextWriter output)
    {
        try
        {
            var updateDto = new MovieUpdateDto
            {
                Title = newTitle,
                Description = newDescription,
                GenreName = newGenre,
                DurationMinutes = duration
            };

            Movie updated = await _movieService.UpdateMovieAsync(currentTitle, updateDto);
            await output.WriteLineAsync($"Successfully updated movie: {updated.Title} (ID: {updated.Id})");
        }
        catch (Exception ex)
        {
            await output.WriteLineAsync($"Failed to update movie: {ex.Message}");
        }
    }
}

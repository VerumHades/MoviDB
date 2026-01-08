using MoviDB.Application.Services;
using MoviDB.Domain.DTOs;
using System;
using System.Collections.Generic;

namespace MoviDB.Presentation.CLI.Commands;

public class ListMoviesCommand : ICommand
{
    private readonly MovieService _movieService;

    public ListMoviesCommand(MovieService movieService)
    {
        _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
    }

    public string Name => "ListMovies";
    public string Description => "Lists movies interactively with table-like display and cursor-based paging.";

    public List<CommandParameter> GetParameters() => new()
    {
        new CommandParameter("batchSize", "Number of movies per page", typeof(int), IsOptional: true)
    };

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        int batchSize = 10;
        if (parameterValues.TryGetValue("batchSize", out var batchObj))
            batchSize = Convert.ToInt32(batchObj);

        (MovieProjection[] Items, MovieCursor? NextCursor) GetBatch(MovieCursor? cursor)
        {
            return _movieService.GetNextBatchOfAllAsync(batchSize, cursor).GetAwaiter().GetResult();
        }

        var pager = new CursorPager<MovieProjection, MovieCursor>(GetBatch, input, output, batchSize);
        pager.Run();
    }
}


using MoviDB.Application.Services;
using MoviDB.Domain.DTOs;

namespace MoviDB.Presentation.CLI.Commands;


public static class MovieCommands
{
    public static ICommand CreateListMoviesCommand(MovieService movieService)
    {
        return new ListBatchCommand<MovieProjection, MovieCursor>(
            name: "ListMovies",
            description: "Lists movies interactively with table-like display and cursor-based paging.",
            getBatchFunc: (batchSize, cursor) =>
                movieService.GetNextBatchOfAllAsync(batchSize, cursor).GetAwaiter().GetResult()
        );
    }
}

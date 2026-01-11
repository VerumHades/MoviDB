using MoviDB.Application.Services;
using MoviDB.Domain.DTOs;

namespace MoviDB.Presentation.CLI.Commands;

public static class SeriesCommands
{
    public static ICommand ListSeriesCommand(SeriesService seriesService)
    {
        return new ListBatchCommand<SeriesProjection, SeriesCursor>(
            name: "ListSeries",
            description: "Lists series interactively with table-like display and cursor-based paging.",
            getBatchFunc: (batchSize, cursor) =>
                seriesService.GetNextBatchAsync(batchSize, cursor, filter: null).GetAwaiter().GetResult()
        );
    }
}
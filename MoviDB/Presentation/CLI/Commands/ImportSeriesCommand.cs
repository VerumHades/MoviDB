using MoviDB.Infrastructure.Serialization;

namespace MoviDB.Presentation.CLI.Commands;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoviDB.Application.DTOs;
using MoviDB.Application.Services;

public class ImportSeriesCommand : ICommand
{
    private readonly SeriesService _seriesService;

    public ImportSeriesCommand(SeriesService seriesService)
    {
        _seriesService = seriesService ?? throw new ArgumentNullException(nameof(seriesService));
    }

    public string Name => "ImportSeries";
    public string Description => "Imports a series with seasons and episodes from a JSON file.";

    public List<CommandParameter> GetParameters()
    {
        return new List<CommandParameter>
        {
            new CommandParameter("filePath", "Path to the JSON file containing series data", typeof(string))
        };
    }

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        if (!parameterValues.TryGetValue("filePath", out var pathObj) || pathObj is not string filePath)
        {
            throw new ArgumentException("Parameter 'filePath' is required and must be a string.");
        }

        ImportSeriesAsync(filePath).GetAwaiter().GetResult();
    }

    private async Task ImportSeriesAsync(string filePath)
    {
        var loader = new SeriesCreationDataJsonLoader();
        SeriesCreationData creationData;

        try
        {
            creationData = loader.LoadFromFile(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load series JSON: {ex.Message}");
            return;
        }

        try
        {
            var series = await _seriesService.RegisterSeriesWithSeasonsAndEpisodesAsync(creationData);
            Console.WriteLine($"Successfully imported series: {series.Title}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to import series: {ex.Message}");
        }
    }
}

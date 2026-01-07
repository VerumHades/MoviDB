using MoviDB.Application.DTOs;

using System.IO;
using System.Text.Json;

namespace MoviDB.Infrastructure.Serialization;

/// <summary>
/// Loads series creation data from a JSON file.
/// </summary>
public sealed class SeriesCreationDataJsonLoader
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    /// <summary>
    /// Initializes the loader with strict JSON settings.
    /// </summary>
    public SeriesCreationDataJsonLoader()
    {
        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false
        };
    }

    /// <summary>
    /// Loads series creation data from the given JSON file path.
    /// </summary>
    /// <param name="filePath">Absolute or relative path to the JSON file.</param>
    /// <returns>Deserialized SeriesCreationData instance.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public SeriesCreationData LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"series creation json file not found: {filePath}");
        }

        var jsonContent = File.ReadAllText(filePath);
        return Deserialize(jsonContent);
    }

    /// <summary>
    /// Deserializes series creation data from a JSON string.
    /// </summary>
    /// <param name="jsonContent">Raw JSON content.</param>
    /// <returns>Deserialized SeriesCreationData instance.</returns>
    /// <exception cref="InvalidDataException"></exception>
    public SeriesCreationData LoadFromJson(string jsonContent)
    {
        return Deserialize(jsonContent);
    }

    /// <summary>
    /// Performs JSON deserialization and validation.
    /// </summary>
    private SeriesCreationData Deserialize(string jsonContent)
    {
        var seriesCreationData = JsonSerializer.Deserialize<SeriesCreationData>(
            jsonContent,
            jsonSerializerOptions
        );

        if (seriesCreationData == null)
        {
            throw new InvalidDataException("failed to deserialize series creation data");
        }

        return seriesCreationData;
    }
}

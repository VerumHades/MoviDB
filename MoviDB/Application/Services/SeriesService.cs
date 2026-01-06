using MoviDB.Application.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.ValueObjects;
using MoviDB.Domain.Views;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public class SeriesService
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public SeriesService(ISeriesRepository seriesRepository, IGenreRepository genreRepository, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _seriesRepository = seriesRepository;
        _genreRepository = genreRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    // ---------------------------
    // Command: Register single series
    // ---------------------------
    public async Task<Series> RegisterSeriesAsync(
        string title,
        string description,
        string genreName)
    {
        var genre = await _genreRepository.GetByNameASync(genreName);
        if (genre is null)
            throw new GenreNotFoundException(genreName);

        var series = Media.CreateSeries(title, description, genre);
        return await _seriesRepository.Create(series);
    }

    // ---------------------------
    // Bulk creation: series with seasons and episodes
    // ---------------------------
    public async Task<Series> RegisterSeriesWithSeasonsAndEpisodesAsync(
        SeriesCreationData creationData)
    {
        // Validate genre
        var genre = await _genreRepository.GetByNameASync(creationData.GenreName);
        if (genre is null)
            throw new GenreNotFoundException(creationData.GenreName);
        
        var uow = _unitOfWorkFactory.Create();

        try
        {
            var series = Media.CreateSeries(creationData.Title, creationData.Description, genre);
            var persistedSeries = await uow.Series.Create(series);
            
            var seasons = creationData.Seasons
                .Select(s => new Season(persistedSeries.Media.Id, s.Number, s.Title))
                .ToList();

            await uow.Series.AddSeasonsBulkAsync(seasons);

            var episodes = creationData.Seasons
                .SelectMany((seasonData, index) =>
                    seasonData.Episodes.Select(e => new Episode(seasons[index].Id, e.Title, e.EpisodeNumber)))
                .ToList();
            
            await uow.Series.AddEpisodesBulkAsync(episodes);
            await uow.CommitAsync();
            
            return persistedSeries;
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }

    public async Task<(SeriesView[], SeriesCursor?)> GetNextBatchAsync(
        int batchSize,
        SeriesCursor? cursor = null,
        SeriesFilter? filter = null)
    {
        return await _seriesRepository.GetNextBatchAsync(batchSize, cursor, filter);
    }

    public async Task<(SeriesEpisodeView[], SeriesEpisodeCursor?)> GetNextBatchEpisodesAsync(
        int batchSize,
        SeriesEpisodeCursor? cursor = null,
        SeriesEpisodeFilter? filter = null)
    {
        return await _seriesRepository.GetNextBatchEpisodesAsync(batchSize, cursor, filter);
    }

    public async Task<List<Season>> GetSeasonsAsync(int seriesId)
    {
        return await _seriesRepository.GetSeasonsAsync(seriesId);
    }

    public async Task<RatingSnapshot> GetRatingSnapshotAsync(int seriesId)
    {
        return await _seriesRepository.GetRatingSnapshotAsync(seriesId);
    }
}

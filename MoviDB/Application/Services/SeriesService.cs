using MoviDB.Application.DTOs;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.ValueObjects;
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
    
    public async Task<Series> RegisterSeriesAsync(
        string title,
        string description,
        string genreName)
    {
        var genre = await _genreRepository.GetByNameAsync(genreName);
        if (genre is null)
            throw new GenreNotFoundException(genreName);

        var series = new Series(title, description, genre);
        return await _seriesRepository.Create(series);
    }

    public async Task<Series> RegisterSeriesWithSeasonsAndEpisodesAsync(
        SeriesCreationData creationData)
    {
        // Validate genre
        var genre = await _genreRepository.GetByNameAsync(creationData.GenreName);
        if (genre is null)
            throw new GenreNotFoundException(creationData.GenreName);
        
        var uow = _unitOfWorkFactory.Create();

        try
        {
            var series = new Series(creationData.Title, creationData.Description, genre);
            var persistedSeries = await uow.Series.Create(series);
            
            var seasons = creationData.Seasons
                .Select(s => new Season(persistedSeries.Id, s.Number, s.Title))
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

    public async Task<(SeriesProjection[], SeriesCursor?)> GetNextBatchAsync(
        int batchSize,
        SeriesCursor? cursor = null,
        SeriesFilter? filter = null)
    {
        return await _seriesRepository.GetNextBatchAsync(batchSize, cursor, filter);
    }

    public async Task<(SeriesEpisodeProjection[], SeriesEpisodeCursor?)> GetNextBatchEpisodesAsync(
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

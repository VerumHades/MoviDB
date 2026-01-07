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
    private readonly ISeriesQueryRepository _seriesQueryRepository;
    private readonly IGenreQueryRepository _genreRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public SeriesService(ISeriesQueryRepository seriesQueryRepository, IGenreQueryRepository genreRepository, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _seriesQueryRepository = seriesQueryRepository;
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

        await using var uow = await _unitOfWorkFactory.Create();

        try
        {
            var series = new Series(title, description, genre);
            var persisted = await uow.Series.Create(series);
            await uow.CommitAsync();
            return persisted;
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }

    public async Task<Series> RegisterSeriesWithSeasonsAndEpisodesAsync(
        SeriesCreationData creationData)
    {
        // Validate genre
        var genre = await _genreRepository.GetByNameAsync(creationData.GenreName);
        if (genre is null)
            throw new GenreNotFoundException(creationData.GenreName);
        
        await using var uow = await _unitOfWorkFactory.Create();

        try
        {
            // Create the series first
            var series = new Series(creationData.Title, creationData.Description, genre);
            var persistedSeries = await uow.Series.Create(series);

            // Add each season individually, collecting the persisted objects
            var persistedSeasons = new List<Season>();
            foreach (var seasonData in creationData.Seasons)
            {
                var season = new Season(persistedSeries.Id, seasonData.Number, seasonData.Title);
                var persistedSeason = await uow.Series.AddSeasonAsync(season);
                persistedSeasons.Add(persistedSeason);
            }

            // Add episodes individually, using the persisted season IDs
            foreach (var (seasonData, index) in creationData.Seasons.Select((s, i) => (s, i)))
            {
                var persistedSeason = persistedSeasons[index];
                foreach (var episodeData in seasonData.Episodes)
                {
                    var episode = new Episode(persistedSeason.Id, episodeData.Title, episodeData.EpisodeNumber);
                    await uow.Series.AddEpisodeAsync(episode);
                }
            }

            // Commit everything in one transaction
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
        return await _seriesQueryRepository.GetNextBatchAsync(batchSize, cursor, filter);
    }

    public async Task<(SeriesEpisodeProjection[], SeriesEpisodeCursor?)> GetNextBatchEpisodesAsync(
        int batchSize,
        SeriesEpisodeCursor? cursor = null,
        SeriesEpisodeFilter? filter = null)
    {
        return await _seriesQueryRepository.GetNextBatchEpisodesAsync(batchSize, cursor, filter);
    }

    public async Task<List<Season>> GetSeasonsAsync(int seriesId)
    {
        return await _seriesQueryRepository.GetSeasonsAsync(seriesId);
    }

    public async Task<RatingSnapshot> GetRatingSnapshotAsync(int seriesId)
    {
        return await _seriesQueryRepository.GetRatingSnapshotAsync(seriesId);
    }
}

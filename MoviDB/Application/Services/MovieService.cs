using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public class MovieService
{
    private readonly IMovieQueryRepository _movieQueryRepository;
    private readonly IGenreQueryRepository _genreRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MovieService(IMovieQueryRepository movieQueryRepository, IGenreQueryRepository genreRepository, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _movieQueryRepository = movieQueryRepository;
        _genreRepository = genreRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Movie> RegisterMovieAsync(
        string title,
        string description,
        string genreName,
        int durationMinutes)
    {
        var genre = await _genreRepository.GetByNameAsync(genreName);
        if (genre == null)
            throw new GenreNotFoundException("Genre not found");
        
        await using var uow = await _unitOfWorkFactory.Create();

        try
        {
            var movie = new Movie(title, description, genre, durationMinutes);
            var persisted = await uow.Movies.Create(movie);
            await uow.CommitAsync();
            return persisted;
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }
    
    public async Task<(MovieProjection[] Movies, MovieCursor? NextCursor)> GetNextBatchOfAllAsync(
        int batchSize,
        MovieCursor? cursor = null,
        MovieFilter? filter = null)
    {
        return await _movieQueryRepository.GetNextBatchOfAllAsync(batchSize, cursor, filter);
    }
}

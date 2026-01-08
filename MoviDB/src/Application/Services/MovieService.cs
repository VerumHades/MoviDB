using MoviDB.Application.DTOs;
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
            throw new GenreNotFoundException(genreName);

        return await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            var movie = new Movie(title, description, genre, durationMinutes);
            var persisted = await uow.Movies.Create(movie);
            return persisted;
        });
    }
    
    public async Task<(MovieProjection[] Movies, MovieCursor? NextCursor)> GetNextBatchOfAllAsync(
        int batchSize,
        MovieCursor? cursor = null,
        MovieFilter? filter = null)
    {
        return await _movieQueryRepository.GetNextBatchOfAllAsync(batchSize, cursor, filter);
    }
    
    public async Task<Movie> UpdateMovieAsync(string movieTitle, MovieUpdateDto updateDto)
    {
        if (string.IsNullOrWhiteSpace(movieTitle))
            throw new ArgumentException("Movie title cannot be empty.", nameof(movieTitle));

        if (updateDto == null)
            throw new ArgumentNullException(nameof(updateDto));
        
        var movie = await _movieQueryRepository.GetByTitleAsync(movieTitle);
        if (movie == null)
            throw new MovieNotFoundException(movieTitle);

        Genre? newGenre = null;
        if (!string.IsNullOrWhiteSpace(updateDto.GenreName))
        {
            newGenre = await _genreRepository.GetByNameAsync(updateDto.GenreName);
            if (newGenre == null)
                throw new GenreNotFoundException(updateDto.GenreName);
        }
        
        if (!string.IsNullOrWhiteSpace(updateDto.Title))
            movie.Title = updateDto.Title;

        if (!string.IsNullOrWhiteSpace(updateDto.Description))
            movie.Description = updateDto.Description;

        if (newGenre != null)
            movie.Genre = newGenre;

        if (updateDto.DurationMinutes.HasValue)
            movie.DurationMinutes = updateDto.DurationMinutes.Value;
        
        return await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
            await uow.Movies.Update(movie)
        );
    }
    
    public async Task DeleteMovieByTitleAsync(string movieTitle)
    {
        if (string.IsNullOrWhiteSpace(movieTitle))
            throw new ArgumentException("Movie title cannot be empty.", nameof(movieTitle));

        var movie = await _movieQueryRepository.GetByTitleAsync(movieTitle);
        if (movie == null)
            throw new MovieNotFoundException(movieTitle);

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.Movies.Delete(movie.Id);
        });
    }
}

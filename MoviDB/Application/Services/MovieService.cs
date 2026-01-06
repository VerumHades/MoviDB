using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;
using MoviDB.Domain.Views;

namespace MoviDB.Application.Services;

public class MovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IGenreRepository _genreRepository;

    public MovieService(IMovieRepository movieRepository, IGenreRepository genreRepository)
    {
        _movieRepository = movieRepository;
        _genreRepository = genreRepository;
    }

    public async Task<Movie> RegisterMovieAsync(
        string title,
        string description,
        string genreName,
        int durationMinutes)
    {
        var genre = await _genreRepository.GetByNameASync(genreName);
        if (genre == null)
            throw new GenreNotFoundException("Genre not found");
        
        var movie = Media.CreateMovie(title, description, genre, durationMinutes);
        return await _movieRepository.Create(movie);
    }
    
    public async Task<(MovieView[] Movies, MovieCursor? NextCursor)> GetNextBatchOfAllAsync(
        int batchSize,
        MovieCursor? cursor = null,
        MovieFilter? filter = null)
    {
        return await _movieRepository.GetNextBatchOfAllAsync(batchSize, cursor, filter);
    }
}

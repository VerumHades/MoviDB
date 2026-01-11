using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Entities.User;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public class UserService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IMovieQueryRepository _movieQueryRepository;
    private readonly ISeriesQueryRepository _seriesQueryRepository;

    public UserService(IUnitOfWorkFactory unitOfWorkFactory, IUserQueryRepository userQueryRepository, IMovieQueryRepository movieQueryRepository, ISeriesQueryRepository seriesQueryRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userQueryRepository = userQueryRepository;
        _movieQueryRepository = movieQueryRepository;
        _seriesQueryRepository = seriesQueryRepository;
    }

    public async Task AddMovieToLibrary(string username, string movieTitle)
    {
        Movie? movie = await _movieQueryRepository.GetByTitleAsync(movieTitle);
        if (movie == null)
            throw new MovieNotFoundException(movieTitle);
        
        User user = await _userQueryRepository.GetByNameAsync(username);

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.UsersLibrary.AddLibraryEntryAsync(user.Id, movie.Id);
        });
    }

    public async Task AddSeriesToLibrary(string username, string seriesTitle)
    {
        Series? series = await _seriesQueryRepository.GetByTitleAsync(seriesTitle);
        if (series == null)
            throw new KeyNotFoundException("Series not found");
        
        User user = await _userQueryRepository.GetByNameAsync(username);

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.UsersLibrary.AddLibraryEntryAsync(user.Id, series.Id);
        });
    }
    
    public async Task MarkMovieWatchedAsync(string username, string movieTitle, bool isWatched)
    {
        Movie? movie = await _movieQueryRepository.GetByTitleAsync(movieTitle);
        if (movie == null)
            throw new MovieNotFoundException(movieTitle);

        User user = await _userQueryRepository.GetByNameAsync(username);

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.UsersLibrary.MarkWatchedStateAsync(user.Id, movie.Id, isWatched);
        });
    }

    public async Task MarkSeriesWatchedAsync(string username, string seriesTitle, bool isWatched)
    {
        Series? series = await _seriesQueryRepository.GetByTitleAsync(seriesTitle);
        if (series == null)
            throw new KeyNotFoundException($"Series not found: {seriesTitle}");

        User user = await _userQueryRepository.GetByNameAsync(username);

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.UsersLibrary.MarkWatchedStateAsync(user.Id, series.Id, isWatched);
        });
    }
}
using MoviDB.Application.DTOs;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Entities.User;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;
using System.Threading.Tasks;

namespace MoviDB.Application.Services;

public sealed class MovieReviewService
{
    private readonly IMovieQueryRepository _movieQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MovieReviewService(
        IMovieQueryRepository movieQueryRepository,
        IUserQueryRepository userQueryRepository,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        _movieQueryRepository = movieQueryRepository;
        _userQueryRepository = userQueryRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task AddReviewAsync(ReviewCreationData creationData)
    {
        Movie? movie = await _movieQueryRepository.GetByTitleAsync(creationData.MediaTitle);
        if (movie == null)
            throw new MovieNotFoundException(creationData.MediaTitle);

        User user = await _userQueryRepository.GetByNameAsync(creationData.Username);

        var review = new Review(
            mediaId: movie.Id,
            userId: user.Id,
            title: creationData.Title,
            content: creationData.Content,
            rating: creationData.Rating
        );

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.Reviews.CreateAsync(review);
        });
    }

    public async Task UpdateReviewAsync(int reviewId, ReviewUpdateData updateData)
    {
        Movie? movie = await _movieQueryRepository.GetByTitleAsync(updateData.MediaTitle);
        if (movie == null)
            throw new MovieNotFoundException(updateData.MediaTitle);

        User user = await _userQueryRepository.GetByNameAsync(updateData.Username);

        var review = new Review(
            mediaId: movie.Id,
            userId: user.Id,
            title: updateData.Title,
            content: updateData.Content,
            rating: updateData.Rating
        );

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.Reviews.UpdateAsync(review);
        });
    }

    public async Task RemoveReviewAsync(string username, string mediaTitle)
    {
        User user = await _userQueryRepository.GetByNameAsync(username);
        Movie? movie = await _movieQueryRepository.GetByTitleAsync(mediaTitle);
        if (movie == null)
            throw new MovieNotFoundException(mediaTitle);

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.Reviews.RemoveAsync(movie.Id, user.Id);
        });
    }
}

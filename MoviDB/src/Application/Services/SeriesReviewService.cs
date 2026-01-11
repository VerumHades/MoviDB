using MoviDB.Application.DTOs;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Entities.User;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;
using System.Threading.Tasks;

namespace MoviDB.Application.Services;

public sealed class SeriesReviewService
{
    private readonly ISeriesQueryRepository _seriesQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public SeriesReviewService(
        ISeriesQueryRepository seriesQueryRepository,
        IUserQueryRepository userQueryRepository,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        _seriesQueryRepository = seriesQueryRepository;
        _userQueryRepository = userQueryRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task AddReviewAsync(ReviewCreationData creationData)
    {
        // Resolve series and user
        Series? series = await _seriesQueryRepository.GetByTitleAsync(creationData.MediaTitle);
        if (series == null)
            throw new KeyNotFoundException($"Series not found: {creationData.MediaTitle}");

        User user = await _userQueryRepository.GetByNameAsync(creationData.Username);

        var review = new Review(
            mediaId: series.Id,
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
        Series? series = await _seriesQueryRepository.GetByTitleAsync(updateData.MediaTitle);
        if (series == null)
            throw new KeyNotFoundException($"Series not found: {updateData.MediaTitle}");

        User user = await _userQueryRepository.GetByNameAsync(updateData.Username);

        var review = new Review(
            mediaId: series.Id,
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

    public async Task RemoveReviewAsync(string username, string seriesTitle)
    {
        User user = await _userQueryRepository.GetByNameAsync(username);
        Series? series = await _seriesQueryRepository.GetByTitleAsync(seriesTitle);

        if (series == null)
            throw new KeyNotFoundException($"Series not found: {seriesTitle}");

        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            // Requires repository support for deletion by userId + mediaId
            await uow.Reviews.RemoveAsync(user.Id, series.Id);
        });
    }
}

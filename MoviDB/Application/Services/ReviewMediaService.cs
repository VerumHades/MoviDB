using MoviDB.Application.DTOs;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public sealed class ReviewMediaService
{
    private readonly IMediaExistenceChecker mediaExistenceChecker;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ReviewMediaService(IMediaExistenceChecker mediaExistenceChecker, IUnitOfWorkFactory unitOfWorkFactory)
    {
        this.mediaExistenceChecker = mediaExistenceChecker;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task AddReviewAsync(ReviewCreationData creationData)
    {
        if (!await mediaExistenceChecker.MediaExistsAsync(creationData.MediaId))
        {
            throw new MediaNotFoundException(creationData.MediaId);
        }

        var review = new Review(
            creationData.MediaId,
            creationData.UserId,
            creationData.Title,
            creationData.Content,
            creationData.Rating);
    
        await using var uow = await _unitOfWorkFactory.Create();

        try
        {
           await uow.Reviews.CreateAsync(review);
           await uow.CommitAsync();
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateReviewAsync(int reviewId, ReviewUpdateData updateData)
    {
        if (!await mediaExistenceChecker.MediaExistsAsync(updateData.MediaId))
        {
            throw new MediaNotFoundException(updateData.MediaId);
        }

        var review = new Review(
            updateData.MediaId,
            updateData.UserId,
            updateData.Title,
            updateData.Content,
            updateData.Rating);
        
        await using var uow = await _unitOfWorkFactory.Create();

        try
        {
            await uow.Reviews.UpdateAsync(review);
            await uow.CommitAsync();
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }

    }

    public async Task RemoveReviewAsync(int reviewId, int userId)
    {
        await using var uow = await _unitOfWorkFactory.Create();

        try
        {
            await uow.Reviews.RemoveAsync(reviewId, userId);
            await uow.CommitAsync();
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }
}
using MoviDB.Application.DTOs;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public sealed class ReviewMediaService
{
    private readonly IReviewCommandRepository reviewCommandRepository;
    private readonly IMediaExistenceChecker mediaExistenceChecker;

    public ReviewMediaService(
        IReviewCommandRepository reviewCommandRepository,
        IMediaExistenceChecker mediaExistenceChecker)
    {
        this.reviewCommandRepository = reviewCommandRepository;
        this.mediaExistenceChecker = mediaExistenceChecker;
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

        await reviewCommandRepository.CreateAsync(review);
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

        await reviewCommandRepository.UpdateAsync(review);
    }

    public async Task RemoveReviewAsync(int reviewId, int userId)
    {
        await reviewCommandRepository.RemoveAsync(reviewId, userId);
    }
}
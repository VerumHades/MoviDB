using MoviDB.Domain.DTOs;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public class ReviewMediaService
{
    private IReviewCommandRepository _reviewCommandRepository;
    private IMediaExistenceChecker _mediaExistenceChecker;

    public ReviewMediaService(IReviewCommandRepository reviewCommandRepository, IMediaExistenceChecker mediaExistenceChecker)
    {
        _reviewCommandRepository = reviewCommandRepository;
        _mediaExistenceChecker = mediaExistenceChecker;
    }

    public async Task AddReviewAsync(ReviewCreationData creationData)
    {
        if (!await _mediaExistenceChecker.MediaExistsAsync(creationData.MediaId))
            throw new MediaNotFoundException(creationData.MediaId);

        await _reviewCommandRepository.AddReviewAsync(creationData);
    }

    public async Task UpdateReviewAsync(ReviewUpdateData updateData, int reviewId)
    {
        if (!await _mediaExistenceChecker.MediaExistsAsync(updateData.MediaId))
            throw new MediaNotFoundException(updateData.MediaId);

        await _reviewCommandRepository.UpdateReviewAsync(updateData);
    }

    public async Task RemoveReviewAsync(int reviewId, int userId)
    {
        await _reviewCommandRepository.RemoveReviewAsync(reviewId, userId);
    }
}
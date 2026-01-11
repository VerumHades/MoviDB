namespace MoviDB.Domain.Repositories;

public interface IUserLibraryCommandRepository
{
    Task AddLibraryEntryAsync(int userId, int mediaId);
    Task MarkWatchedStateAsync(int userId, int mediaId, bool isWatched);
}

public interface IUserLibraryQueryRepository
{
    Task<bool> LibraryEntryExistsAsync(int userId, int mediaId);
}
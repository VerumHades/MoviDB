namespace MoviDB.Domain.Repositories;

public interface IUserLibraryCommandRepository
{
    Task AddLibraryEntryAsync(int userId, int mediaId);
    Task MarkWatchedAsync(int userId, int mediaId);
}

public interface IUserLibraryQueryRepository
{
    Task<bool> LibraryEntryExistsAsync(int userId, int mediaId);
}
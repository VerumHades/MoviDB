namespace MoviDB.Domain.Repositories;

public interface IUserLibraryRepository
{
    Task AddLibraryEntryAsync(int userId, int mediaId);
    Task MarkWatchedAsync(int userId, int mediaId);
    Task<bool> LibraryEntryExistsAsync(int userId, int mediaId);
}
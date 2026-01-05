using MoviDB.domain.entities;

namespace MoviDB.domain.repositories;

public interface ILibraryEntryRepository
{
    LibraryEntry Get(int mediaId, int userId);

    LibraryEntry Add(int mediaId, int userId, bool watched);

    void MarkWatched(int mediaId, int userId);
    void MarkUnwatched(int mediaId, int userId);

    /// <summary>
    /// Fetch library entries for a user, cursor-paged.
    /// </summary>
    IReadOnlyList<LibraryEntry> GetByUserId(int userId, int pageSize, int? cursorAfterId = null);
}
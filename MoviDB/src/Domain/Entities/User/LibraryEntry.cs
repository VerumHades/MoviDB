using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.User;

public sealed class LibraryEntry: Entity
{
    public int MediaId { get; }
    public int UserId { get; }
    public bool Watched { get; private set; }
    public LibraryEntry(int mediaId, int userId, bool watched)
    {
        MediaId = mediaId;
        UserId = userId;
        Watched = watched;
    }
}
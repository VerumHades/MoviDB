namespace MoviDB.domain.entities;

public sealed class LibraryEntry
{
    public int MediaId { get; }
    public int UserId { get; }
    public bool Watched { get; private set; }
    public DateTime CreatedAt { get; }

    public LibraryEntry(int mediaId, int userId, bool watched, DateTime createdAt)
    {
        MediaId = mediaId;
        UserId = userId;
        Watched = watched;
        CreatedAt = createdAt;
    }

    public void MarkWatched() => Watched = true;
    public void MarkUnwatched() => Watched = false;
}
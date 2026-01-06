namespace MoviDB.Domain.Exceptions;

public class MediaNotFoundException : Exception
{
    public int MediaId { get; }

    public MediaNotFoundException(int mediaId)
        : base($"Media with ID {mediaId} does not exist.")
    {
        MediaId = mediaId;
    }
}
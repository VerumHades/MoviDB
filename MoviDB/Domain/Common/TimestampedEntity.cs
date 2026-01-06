namespace MoviDB.Domain.Common;

public class TimestampedEntity: Entity
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
namespace MoviDB.Domain.Common;


/// <summary>
/// Base entity with a database Id.
/// </summary>
public abstract class Entity(int id = -1)
{
    /// <summary>
    /// Gets or sets the primary key for the entity.
    /// </summary>
    public int Id { get; protected set; } = id;
}
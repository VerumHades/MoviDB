namespace MoviDB.Infrastructure.Database;

/// <summary>
/// Represents a mapping between a property of a filter object and a SQL condition.
/// </summary>
/// <typeparam name="TFilter">Type of the filter object.</typeparam>
public class FilterMapping<TFilter>
{
    /// <summary>
    /// The column name in the database.
    /// </summary>
    public string Column { get; init; }

    /// <summary>
    /// SQL operator to use (e.g., '=', '>=', 'LIKE').
    /// </summary>
    public string Operator { get; init; }

    /// <summary>
    /// Function to extract the value from the filter object.
    /// </summary>
    public Func<TFilter, object?> ValueSelector { get; init; }

    /// <summary>
    /// Optional transformation to apply to the value before adding as a parameter.
    /// </summary>
    public Func<object, object>? Transform { get; init; }
}

using System.Text;

namespace MoviDB.Infrastructure.Database;

public static class FilterMapper
{
    /// <summary>
    /// Builds a SQL WHERE and ORDER BY clause with parameters from a cursor and generic filter mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter object.</typeparam>
    /// <param name="cursorCreatedAt">Optional cursor timestamp for keyset pagination.</param>
    /// <param name="cursorId">Optional cursor ID for keyset pagination.</param>
    /// <param name="filter">Filter object of type TFilter.</param>
    /// <param name="mappings">List of reusable filter mappings.</param>
    /// <returns>A tuple containing the SQL clause and dictionary of parameters.</returns>
    public static (string SqlClause, Dictionary<string, object> Parameters) BuildQueryClause<TFilter>(
        DateTime? cursorCreatedAt,
        long? cursorId,
        TFilter? filter,
        IReadOnlyList<FilterMapping<TFilter>>? mappings)
    {
        var parameters = new Dictionary<string, object>();
        var conditions = new List<string>();

        if (cursorCreatedAt.HasValue && cursorId.HasValue)
        {
            conditions.Add("(created_at > @cursorCreatedAt OR (created_at = @cursorCreatedAt AND id > @cursorId))");
            parameters["@cursorCreatedAt"] = cursorCreatedAt.Value;
            parameters["@cursorId"] = cursorId.Value;
        }

        if (filter != null && mappings != null)
        {
            foreach (var mapping in mappings)
            {
                var value = mapping.ValueSelector(filter);
                if (value == null || value is string s && string.IsNullOrWhiteSpace(s))
                    continue;

                var paramName = $"@{mapping.Column}{parameters.Count}";
                var finalValue = mapping.Transform != null ? mapping.Transform(value) : value;

                conditions.Add($"{mapping.Column} {mapping.Operator} {paramName}");
                parameters[paramName] = finalValue!;
            }
        }

        var sqlBuilder = new StringBuilder();

        if (conditions.Count > 0)
        {
            sqlBuilder.Append(" WHERE ");
            sqlBuilder.Append(string.Join(" AND ", conditions));
        }

        sqlBuilder.Append(" ORDER BY created_at, id");

        return (sqlBuilder.ToString(), parameters);
    }

    public static (string SqlClause, Dictionary<string, object> Parameters) BuildBatchClause(
        DateTime? cursorCreatedAt,
        long? cursorId)
    {
        return BuildQueryClause<EmptyFilter>(cursorCreatedAt, cursorId, null, null);
    }

}
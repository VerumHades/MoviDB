using MoviDB.Domain.DTOs;

namespace MoviDB.Infrastructure.Database;

public static class MovieFilterMappings
{
    public static readonly List<FilterMapping<MovieFilter>> Mappings = new()
    {
        new FilterMapping<MovieFilter>
        {
            Column = "title",
            Operator = "LIKE",
            ValueSelector = f => f.TitleContains,
            Transform = v => $"%{v}%"
        },
        new FilterMapping<MovieFilter>
        {
            Column = "genre_id",
            Operator = "=",
            ValueSelector = f => f.GenreId
        },
        new FilterMapping<MovieFilter>
        {
            Column = "rating",
            Operator = ">=",
            ValueSelector = f => f.MinRating
        },
        new FilterMapping<MovieFilter>
        {
            Column = "rating",
            Operator = "<=",
            ValueSelector = f => f.MaxRating
        },
        new FilterMapping<MovieFilter>
        {
            Column = "duration_minutes",
            Operator = ">=",
            ValueSelector = f => f.MinDuration
        },
        new FilterMapping<MovieFilter>
        {
            Column = "duration_minutes",
            Operator = "<=",
            ValueSelector = f => f.MaxDuration
        },
        new FilterMapping<MovieFilter>
        {
            Column = "created_at",
            Operator = ">=",
            ValueSelector = f => f.CreatedAfter
        },
        new FilterMapping<MovieFilter>
        {
            Column = "created_at",
            Operator = "<=",
            ValueSelector = f => f.CreatedBefore
        }
    };
}

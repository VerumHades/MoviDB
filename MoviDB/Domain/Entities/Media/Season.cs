using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.Media;

public sealed class Season: Entity
{
    public int Id { get; }
    public int SeriesMediaId { get; }
    public string Title { get; }
    public int Number { get; }

    public Season(int seriesMediaId, int number, string title)
    {
        if (number <= 0) throw new ArgumentException("Season number must be positive");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Season title cannot be empty");

        SeriesMediaId = seriesMediaId;
        Number = number;
        Title = title;
    }
}
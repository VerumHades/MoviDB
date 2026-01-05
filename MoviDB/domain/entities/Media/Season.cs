namespace MoviDB.domain.entities;

public sealed class Season
{
    public int Id { get; }
    public int SeriesMediaId { get; }
    public string Title { get; }
    public int Number { get; }

    public Season(int id, int seriesMediaId, int number, string title)
    {
        if (number <= 0) throw new ArgumentException("Season number must be positive");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Season title cannot be empty");

        Id = id;
        SeriesMediaId = seriesMediaId;
        Number = number;
        Title = title;
    }
}
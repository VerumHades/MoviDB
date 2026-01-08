namespace MoviDB.Domain.ValueObjects;

public sealed class RatingSnapshot
{
    public int RatingCount { get; }
    public double AverageRating { get; }

    public RatingSnapshot(int ratingCount, double averageRating)
    {
        if (ratingCount < 0) throw new ArgumentOutOfRangeException(nameof(ratingCount));
        if (averageRating < 0 || averageRating > 10) throw new ArgumentOutOfRangeException(nameof(averageRating));

        RatingCount = ratingCount;
        AverageRating = averageRating;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not RatingSnapshot other) return false;
        return RatingCount == other.RatingCount && AverageRating.Equals(other.AverageRating);
    }

    public override int GetHashCode() => HashCode.Combine(RatingCount, AverageRating);
    
    public override string ToString()
    {
        return $"Rating: {AverageRating:F2} ({RatingCount} votes)";
    }
}

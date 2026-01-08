using MoviDB.Presentation.CLI;


/// <summary>
/// Constraint that ensures an integer parameter is within a specified range [Min, Max].
/// </summary>
public class IntegerRangeConstraint : IParameterConstraint
{
    public int Min { get; }
    public int Max { get; }

    public IntegerRangeConstraint(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("Min cannot be greater than Max.", nameof(min));

        Min = min;
        Max = max;
    }

    public bool IsValid(string value)
    {
        if (!int.TryParse(value, out var intValue))
            return false;

        return intValue >= Min && intValue <= Max;
    }

    public string Description => $"Value must be an integer between {Min} and {Max}.";
}
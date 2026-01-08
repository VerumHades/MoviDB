namespace MoviDB.Presentation.CLI.Constraints;


/// <summary>
/// Example: Constraint enforcing minimum and maximum string length.
/// </summary>
public class StringLengthConstraint : IParameterConstraint
{
    public int MinLength { get; }
    public int MaxLength { get; }

    public StringLengthConstraint(int minLength, int maxLength)
    {
        MinLength = minLength;
        MaxLength = maxLength;
    }

    public bool IsValid(string value) => value.Length >= MinLength && value.Length <= MaxLength;

    public string Description => $"Value must be between {MinLength} and {MaxLength} characters long";
}
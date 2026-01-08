namespace MoviDB.Presentation.CLI;

/// <summary>
/// Interface for a parameter constraint.
/// Implementations define validation logic and provide a description for help/error messages.
/// </summary>
public interface IParameterConstraint
{
    /// <summary>
    /// Checks if the value is valid according to this constraint.
    /// </summary>
    /// <param name="value">Parameter value as string.</param>
    /// <returns>True if valid, false otherwise.</returns>
    bool IsValid(string value);

    /// <summary>
    /// Description of the constraint for help or error messages.
    /// </summary>
    string Description { get; }
}
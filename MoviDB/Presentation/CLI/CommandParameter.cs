namespace MoviDB.Presentation.CLI;


/// <summary>
/// Represents a single command parameter with optional multiple constraints.
/// </summary>
public record CommandParameter(
    string Name,
    string Description,
    Type ParameterType,
    bool IsOptional = false,
    IReadOnlyList<IParameterConstraint> Constraints = null)
{
    public IReadOnlyList<IParameterConstraint> Constraints { get; init; } = Constraints ?? Array.Empty<IParameterConstraint>();
}
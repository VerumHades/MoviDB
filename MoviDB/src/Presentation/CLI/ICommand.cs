namespace MoviDB.Presentation.CLI;

/// <summary>
/// Represents a generic command with name, description, and parameters.
/// </summary>
public interface ICommand
{
    string Name { get; }
    string Description { get; }

    /// <summary>
    /// Returns the parameters required for this command.
    /// </summary>
    List<CommandParameter> GetParameters();

    /// <summary>
    /// Executes the command.
    /// </summary>
    void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output);
}
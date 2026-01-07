namespace MoviDB.Presentation.CLI;

/// <summary>
/// Command invoker or registry.
/// </summary>
public class CommandRegistry
{
    private readonly Dictionary<string, ICommand> _commands = new();

    public void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
    }

    public void ExecuteCommand(string name, Dictionary<string, object> parameters)
    {
        if (_commands.TryGetValue(name, out var command))
        {
            command.Execute(parameters);
        }
        else
        {
            throw new InvalidOperationException($"Command '{name}' not found.");
        }
    }

    public IEnumerable<ICommand> GetAllCommands() => _commands.Values;
}
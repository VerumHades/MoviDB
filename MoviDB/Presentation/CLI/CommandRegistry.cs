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
    
    public IReadOnlyDictionary<string, ICommand> Commands => _commands;
    public IEnumerable<ICommand> GetAllCommands() => _commands.Values;
}
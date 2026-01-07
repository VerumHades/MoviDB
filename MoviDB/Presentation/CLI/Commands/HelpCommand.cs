namespace MoviDB.Presentation.CLI.Commands;

using System;
using System.Collections.Generic;

/// <summary>
/// Command that lists all registered commands and their parameters.
/// </summary>
public class HelpCommand : ICommand
{
    private readonly CommandRegistry _registry;

    public HelpCommand(CommandRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public string Name => "Help";
    public string Description => "Lists all available commands with descriptions and parameters.";

    public List<CommandParameter> GetParameters()
    {
        return new List<CommandParameter>();
    }

    public void Execute(Dictionary<string, object> parameterValues)
    {
        Console.WriteLine("Available commands:\n");

        foreach (var command in _registry.GetAllCommands())
        {
            Console.WriteLine($"Command: {command.Name}");
            Console.WriteLine($"Description: {command.Description}");

            var parameters = command.GetParameters();
            if (parameters.Count > 0)
            {
                Console.WriteLine("Parameters:");
                foreach (var param in parameters)
                {
                    Console.WriteLine($"  - {param.Name} ({param.ParameterType.Name}): {param.Description}");
                }
            }
            else
            {
                Console.WriteLine("Parameters: None");
            }

            Console.WriteLine();
        }
    }
}

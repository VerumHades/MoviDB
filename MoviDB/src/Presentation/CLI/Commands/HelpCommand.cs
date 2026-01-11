namespace MoviDB.Presentation.CLI.Commands;

using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Command that lists all registered commands and their parameters, or details for a specific command.
/// </summary>
public class HelpCommand : ICommand
{
    private readonly CommandRegistry _registry;

    public HelpCommand(CommandRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public string Name => "Help";
    public string Description => "Lists all available commands or details of a specific command.";

    public List<CommandParameter> GetParameters() => new()
    {
        new CommandParameter(
            Name: "command",
            Description: "Optional. The name of the command to display detailed help for.",
            ParameterType: typeof(string),
            IsOptional: true
        )
    };

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        if (parameterValues.TryGetValue("command", out var commandObj) && commandObj is string commandName)
        {
            // Show help for a specific command
            if (!_registry.Commands.TryGetValue(commandName, out var command))
            {
                output.WriteLine($"Command '{commandName}' not found.");
                return;
            }

            PrintCommandHelp(command, output);
        }
        else
        {
            // Show help for all commands
            output.WriteLine("Available commands:\n");

            foreach (var command in _registry.GetAllCommands())
            {
                PrintCommandHelp(command, output);
                output.WriteLine();
            }
        }
    }

    private void PrintCommandHelp(ICommand command, TextWriter output)
    {
        output.WriteLine($"Command: {command.Name}");
        output.WriteLine($"Description: {command.Description}");

        var parameters = command.GetParameters();
        if (parameters.Count > 0)
        {
            output.WriteLine("Parameters:");
            foreach (var param in parameters)
            {
                var optionalText = param.IsOptional ? "Optional" : "Required";
                output.WriteLine($"  - {param.Name} ({param.ParameterType.Name}) [{optionalText}]: {param.Description}");

                if (param.Constraints.Count > 0)
                {
                    output.WriteLine("    Constraints:");
                    foreach (var constraint in param.Constraints)
                    {
                        output.WriteLine($"      - {constraint.Description}");
                    }
                }
            }
        }
        else
        {
            output.WriteLine("Parameters: None");
        }
    }
}

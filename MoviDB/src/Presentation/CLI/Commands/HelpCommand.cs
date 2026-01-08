namespace MoviDB.Presentation.CLI.Commands;

using System;
using System.Collections.Generic;

/// <summary>
/// Command that lists all registered commands and their parameters, including constraints and optional flags.
/// </summary>
public class HelpCommand : ICommand
{
    private readonly CommandRegistry _registry;

    public HelpCommand(CommandRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public string Name => "Help";
    public string Description => "Lists all available commands with descriptions, parameters, and constraints.";

    public List<CommandParameter> GetParameters() => new();

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        output.WriteLine("Available commands:\n");

        foreach (var command in _registry.GetAllCommands())
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

            output.WriteLine();
        }
    }
}
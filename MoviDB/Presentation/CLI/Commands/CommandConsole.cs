using System;
using System.Collections.Generic;
using System.Linq;
using MoviDB.Presentation.CLI;

/// <summary>
/// Simple synchronous console for executing commands from a registry.
/// </summary>
public class CommandConsole
{
    private readonly CommandRegistry _registry;

    public CommandConsole(CommandRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Starts the interactive console loop.
    /// </summary>
    public void Run()
    {
        Console.WriteLine("Welcome to MoviDB CLI. Type 'Help' to see available commands.\n");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            try
            {
                ExecuteInput(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine("Goodbye!");
    }

    private void ExecuteInput(string input)
    {
        // Split command name and arguments
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandName = parts[0];
        var argPairs = parts.Skip(1);

        // Build parameter dictionary
        var parameters = new Dictionary<string, object>();
        foreach (var arg in argPairs)
        {
            var kv = arg.Split('=', 2);
            if (kv.Length != 2)
            {
                Console.WriteLine($"Skipping invalid parameter: {arg}");
                continue;
            }
            parameters[kv[0]] = kv[1];
        }

        // Find command
        var command = _registry.GetAllCommands()
                               .FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

        if (command == null)
        {
            Console.WriteLine($"Command '{commandName}' not found. Type 'Help' to see available commands.");
            return;
        }

        // Execute synchronously
        command.Execute(parameters);
    }
}

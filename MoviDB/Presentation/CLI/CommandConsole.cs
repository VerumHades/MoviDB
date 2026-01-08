using MoviDB.Presentation.CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Stream-based CLI for executing commands from a registry.
/// Supports parameter validation using constraints.
/// </summary>
public class CommandConsole
{
    private readonly CommandRegistry _registry;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public CommandConsole(CommandRegistry registry, TextReader input, TextWriter output)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void Run()
    {
        _output.WriteLine("Welcome to MoviDB CLI. Type 'Help' to see available commands.\n");

        while (true)
        {
            _output.Write("> ");
            var line = _input.ReadLine();
            if (line == null || line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                ExecuteInput(line);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error: {ex.Message}");
            }
        }

        _output.WriteLine("Goodbye!");
    }

    private void ExecuteInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        var parts = SplitInput(input);
        if (parts.Length == 0)
            return;

        var commandName = parts[0];

        if (!_registry.Commands.TryGetValue(commandName, out var command))
        {
            _output.WriteLine($"Command '{commandName}' not found. Type 'Help' to see available commands.");
            return;
        }

        var parameters = ParseParameters(parts.Skip(1));


        if (!TryValidateAndConvertParameters(command.GetParameters(), parameters, out var convertedParameters, out var errors))
        {
            _output.WriteLine("Parameter validation failed:");
            foreach (var error in errors)
            {
                _output.WriteLine($" - {error}");
            }
            return;
        }

        command.Execute(convertedParameters, _input, _output);
    }

    /// <summary>
    /// Splits the input into arguments, respecting quoted strings.
    /// </summary>
    private static string[] SplitInput(string input)
    {
        var args = new List<string>();
        bool inQuotes = false;
        var current = new StringBuilder();

        foreach (char c in input)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    args.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            args.Add(current.ToString());

        return args.ToArray();
    }

    /// <summary>
    /// Converts arguments of the form key=value into a dictionary.
    /// </summary>
    private Dictionary<string, object> ParseParameters(IEnumerable<string> argPairs)
    {
        var parameters = new Dictionary<string, object>();

        foreach (var arg in argPairs)
        {
            var kv = arg.Split('=', 2);
            if (kv.Length != 2)
            {
                _output.WriteLine($"Skipping invalid parameter: {arg}");
                continue;
            }

            parameters[kv[0]] = kv[1];
        }

        return parameters;
    }

    /// <summary>
    /// Validates parameters against requiredness, type, and constraints,
    /// and returns a dictionary of converted values if valid.
    /// Throws an exception or returns errors if validation fails.
    /// </summary>
    private bool TryValidateAndConvertParameters(
        IEnumerable<CommandParameter> commandParameters,
        Dictionary<string, object> suppliedParameters,
        out Dictionary<string, object> convertedParameters,
        out List<string> errors)
    {
        convertedParameters = new Dictionary<string, object>();
        errors = new List<string>();

        foreach (var param in commandParameters)
        {
            if (!param.IsOptional && !suppliedParameters.ContainsKey(param.Name))
            {
                errors.Add($"Missing required parameter '{param.Name}'.");
                continue;
            }

            if (!suppliedParameters.TryGetValue(param.Name, out var rawValue))
                continue; 

            var valueStr = rawValue?.ToString() ?? string.Empty;


            if (!TryConvertToType(valueStr, param.ParameterType, out var convertedValue))
            {
                errors.Add($"Parameter '{param.Name}' must be of type {param.ParameterType.Name}.");
                continue;
            }

            foreach (var constraint in param.Constraints)
            {
                if (!constraint.IsValid(valueStr))
                {
                    errors.Add($"Parameter '{param.Name}' invalid: {constraint.Description}");
                }
            }
            if (!errors.Exists(e => e.Contains($"'{param.Name}'"))) 
            {
                convertedParameters[param.Name] = convertedValue!;
            }
        }

        return errors.Count == 0;
    }
    
    bool TryConvertToType(string valueStr, Type targetType, out object? converted)
    {
        converted = null;

        if (string.IsNullOrEmpty(valueStr) && Nullable.GetUnderlyingType(targetType) != null)
        {
            // Null is OK for nullable types
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            if (underlyingType.IsEnum)
            {
                converted = Enum.Parse(underlyingType, valueStr, ignoreCase: true);
            }
            else
            {
                converted = Convert.ChangeType(valueStr, underlyingType);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}

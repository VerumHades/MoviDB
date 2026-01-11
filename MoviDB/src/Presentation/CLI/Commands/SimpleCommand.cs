using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoviDB.Presentation.CLI.Commands;

/// <summary>
/// Generic command for executing a simple service action with parameters and output.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the action.</typeparam>
public class SimpleCommand<TResult> : ICommand
{
    private readonly Func<Dictionary<string, object>, TextWriter, Task<TResult>> _action;

    public SimpleCommand(
        string name,
        string description,
        List<CommandParameter> parameters,
        Func<Dictionary<string, object>, TextWriter, Task<TResult>> action)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Parameters = parameters ?? new List<CommandParameter>();
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public string Name { get; }

    public string Description { get; }

    public List<CommandParameter> Parameters { get; }

    public List<CommandParameter> GetParameters() => Parameters;

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        try
        {
            var result = _action(parameterValues, output).GetAwaiter().GetResult();
            if (result != null)
            {
                output.WriteLine(result.ToString());
            }
        }
        catch (Exception ex)
        {
            output.WriteLine($"Command failed: {ex.Message}");
        }
    }
}

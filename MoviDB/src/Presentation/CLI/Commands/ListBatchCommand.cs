using MoviDB.Application.Services;
using MoviDB.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.IO;

namespace MoviDB.Presentation.CLI.Commands;

/// <summary>
/// Generic command for listing items in a paged, table-like display using cursor-based pagination.
/// Only requires name, description, and the batch retrieval function.
/// </summary>
/// <typeparam name="TItem">The type of items to list.</typeparam>
/// <typeparam name="TCursor">The type of cursor used for pagination.</typeparam>
public class ListBatchCommand<TItem, TCursor> : ICommand
{
    private readonly Func<int, TCursor?, (TItem[] Items, TCursor? NextCursor)> _getBatchFunc;

    public ListBatchCommand(
        string name,
        string description,
        Func<int, TCursor?, (TItem[] Items, TCursor? NextCursor)> getBatchFunc)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        _getBatchFunc = getBatchFunc ?? throw new ArgumentNullException(nameof(getBatchFunc));
    }

    public string Name { get; }

    public string Description { get; }

    public List<CommandParameter> GetParameters() => new()
    {
        new CommandParameter("batchSize", "Number of items per page", typeof(int), IsOptional: true)
    };

    public void Execute(Dictionary<string, object> parameterValues, TextReader input, TextWriter output)
    {
        int batchSize = 10;
        if (parameterValues.TryGetValue("batchSize", out var batchObj))
            batchSize = Convert.ToInt32(batchObj);

        (TItem[] Items, TCursor? NextCursor) GetBatch(TCursor? cursor)
        {
            return _getBatchFunc(batchSize, cursor);
        }

        var pager = new CursorPager<TItem, TCursor>(GetBatch, input, output, batchSize);
        pager.Run();
    }
}

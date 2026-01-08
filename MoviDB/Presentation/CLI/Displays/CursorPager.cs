using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// Interactive cursor-based pager for any item type and cursor type, using streams for I/O.
/// Fetches pages on demand and keeps track of cursors for navigation.
/// </summary>
/// <typeparam name="TItem">Type of items to display.</typeparam>
/// <typeparam name="TCursor">Type of the cursor used for pagination.</typeparam>
public class CursorPager<TItem, TCursor>
{
    private readonly Func<TCursor?, (TItem[] Items, TCursor? NextCursor)> _getBatchFunc;
    private readonly int _pageSize;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    private readonly List<TCursor?> _cursors = new(); // index = page number
    private int _currentPage = 0;

    public CursorPager(
        Func<TCursor?, (TItem[] Items, TCursor? NextCursor)> getBatchFunc,
        TextReader input,
        TextWriter output,
        int pageSize = 10)
    {
        _getBatchFunc = getBatchFunc ?? throw new ArgumentNullException(nameof(getBatchFunc));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _pageSize = Math.Max(1, pageSize);
    }

    public void Run()
    {
        TCursor? nextCursor = default;
        bool firstFetch = true;

        while (true)
        {
            // Determine cursor for current page
            TCursor? cursorToFetch = default(TCursor?);
            if (_currentPage < _cursors.Count)
                cursorToFetch = _cursors[_currentPage];
            else if (firstFetch)
                cursorToFetch = default;
            else
                cursorToFetch = nextCursor;

            // Fetch batch
            var (items, fetchedNextCursor) = _getBatchFunc(cursorToFetch);
            firstFetch = false;

            if (_currentPage >= _cursors.Count)
                _cursors.Add(cursorToFetch);

            DisplayPage(items);

            if (fetchedNextCursor != null && _currentPage == _cursors.Count - 1)
                nextCursor = fetchedNextCursor;
            else
                nextCursor = default;

            // Command input
            _output.WriteLine("\nCommands: [n] Next, [p] Previous, [e] Exit");
            _output.Write("PagedReader > ");
            var input = _input.ReadLine()?.Trim().ToLowerInvariant();

            switch (input)
            {
                case "n":
                    if (!EqualityComparer<TCursor?>.Default.Equals(nextCursor, default) || _currentPage + 1 < _cursors.Count)
                        if(items.Length != 0) _currentPage++;
                    else
                        _output.WriteLine("Already at the last page.");
                    break;
                case "p":
                    if (_currentPage > 0)
                        _currentPage--;
                    else
                        _output.WriteLine("Already at the first page.");
                    break;
                case "e":
                    return;
                default:
                    _output.WriteLine("Invalid command. Use n, p, or e.");
                    break;
            }
        }
    }

    private void DisplayPage(TItem[] items)
    {
        if (!items.Any())
        {
            _output.WriteLine("[No items to display]");
            return;
        }

        var properties = typeof(TItem).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.CanRead)
                                      .ToArray();

        var columnWidths = new int[properties.Length];
        for (int i = 0; i < properties.Length; i++)
        {
            var header = properties[i].Name;
            int maxDataLength = items.Max(item =>
            {
                var value = properties[i].GetValue(item)?.ToString() ?? "";
                return value.Length;
            });
            columnWidths[i] = Math.Max(header.Length, maxDataLength) + 2;
        }

        // Print header
        for (int i = 0; i < properties.Length; i++)
            _output.Write(properties[i].Name.PadRight(columnWidths[i]));
        _output.WriteLine();

        // Separator
        for (int i = 0; i < properties.Length; i++)
            _output.Write(new string('-', columnWidths[i]));
        _output.WriteLine();

        // Rows
        foreach (var item in items)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(item)?.ToString() ?? "";
                _output.Write(value.PadRight(columnWidths[i]));
            }
            _output.WriteLine();
        }

        _output.WriteLine($"Page {_currentPage + 1}");
    }
}

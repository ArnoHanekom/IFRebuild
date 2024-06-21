using Infinity.Services.Interfaces;
using Infinity.Data.Models;
using System.Collections.Concurrent;

namespace Infinity.Services.Services;

public class ConcurrentSearchService : IConcurrentSearchService
{
    private ConcurrentBag<Table> _spinResults { get; set; } = default!;
    public void NewSpinSearch() => _spinResults = [];

    public ConcurrentBag<Table> GetSpinResults()
    {
        _spinResults ??= [];
        lock (_spinResults)
        {
            return _spinResults;
        }
    }
    public void AddSpinResult(Table table)
    {
        _spinResults ??= [];
        lock (_spinResults)
        {
            if (!_spinResults.Any(sr => sr.TableId == table.TableId && sr.Autoplay == table.Autoplay))
            {
                _spinResults.Add(table);
            }
        }
    }
    public Table? GetSpinResultTable(int tableId, int autoplay)
    {
        _spinResults ??= [];
        lock (_spinResults)
        {
            return _spinResults.FirstOrDefault(sr => sr.TableId == tableId && sr.Autoplay == autoplay);
        }
    }
    public ConcurrentBag<Table> GetAllSpinResults()
    {
        _spinResults ??= [];
        lock (_spinResults)
        {
            return [.. _spinResults.OrderByDescending(sr => sr.Rows)];
        }
    }

    public void MarkAllAsDone()
    {
        _spinResults ??= [];
        lock (_spinResults)
        {
            _spinResults.Where(t => !t.DoneSpinning)
                .ToList()
                .ForEach(t => t.DoneSpinning = true);
        }
    }
}
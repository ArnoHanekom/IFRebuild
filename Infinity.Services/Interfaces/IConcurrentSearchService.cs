using System.Collections.Concurrent;
using Infinity.Data.Models;

namespace Infinity.Services.Interfaces;

public interface IConcurrentSearchService
{
    void NewSpinSearch();

    ConcurrentBag<Table> GetSpinResults();

    void AddSpinResult(Table table);

    Table? GetSpinResultTable(int tableId, int autoplay);

    ConcurrentBag<Table> GetAllSpinResults();
    void MarkAllAsDone();
}
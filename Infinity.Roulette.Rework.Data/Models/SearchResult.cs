namespace Infinity.Roulette.Rework.Data.Models;
public class SearchResult
{
    private readonly object _lock = new();
    public Guid Id { get; init; }
    private GameTable _gameTable { get; set; } = default!;
    public GameTable Table
    {
        get
        {
            lock (_lock)
            {
                return _gameTable;
            }
        }
    }

    public SearchResult()
    {
        Id = Guid.NewGuid();
    }

    public void SetResultTable(GameTable table)
    {
        lock ( _lock)
        {
            _gameTable = table;
        }
    }
}
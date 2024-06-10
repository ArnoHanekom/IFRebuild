using Infinity.Roulette.Rework.Data.Models;
using Infinity.Roulette.Rework.Services;
using System.Collections.ObjectModel;

namespace Infinity.Roulette.ViewModels;

public class ReworkResultsViewModel : ViewModelBase
{
    private readonly IPlayService _playService;
    private readonly object _lock = new();
    private SearchResult[] _searchResults { get; set; } = Array.Empty<SearchResult>();
    public SearchResult[] SearchResults
    {
        get { return _searchResults; }
    }
    public GameTable[] DisplayResults { get; set; } = default!;

    private ObservableCollection<GameTable> _gridResults { get; set; } = default!;
    public ObservableCollection<GameTable> GridResults
    {
        get { return _gridResults; }
        set { _gridResults = value; }
    }

    public ReworkResultsViewModel(IPlayService playService)
    {
        _playService = playService;
    }

    public async Task LoadResultsAsync(CancellationToken ct = default)
    {
        _searchResults = await _playService.GetResultsAsync(ct);
    }

    public async Task LoadOrderedResultsForDisplayAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(LoadOrderedResultsForDisplay, ct);
        }
        catch
        {
            LoadOrderedResultsForDisplay();
        }
    }

    private void LoadOrderedResultsForDisplay()
    {
        lock (_lock)
        {
            GridResults = new ObservableCollection<GameTable>(_searchResults.OrderByDescending(sr => sr.Table.Rows).Select(sr => sr.Table).ToList());
        }
    }

    public async Task<GameTable[]> CheckAll(CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            foreach (var result in DisplayResults)
            {
                result.RunSpinfile = true;
            }

            return DisplayResults;
        });
    }

    public async Task LoadCheckAllListAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(LoadCheckAllList, ct);
        }
        catch
        {
            LoadCheckAllList();
        }
    }

    private void LoadCheckAllList()
    {
        lock (_lock)
        {
            GridResults = new ObservableCollection<GameTable>(GridResults.Select(gr =>
            {
                gr.RunSpinfile = true;
                return gr;
            }));
        }
    }

    public async Task LoadUnCheckAllListAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(LoadUncheckAllList, ct);
        }
        catch
        {
            LoadUncheckAllList();
        }
    }

    private void LoadUncheckAllList()
    {
        lock (_lock)
        {
            GridResults = new ObservableCollection<GameTable>(GridResults.Select(gr =>
            {
                gr.RunSpinfile = false;
                return gr;
            }));
        }
    }
}
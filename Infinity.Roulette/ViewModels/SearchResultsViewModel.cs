using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using Infinity.Engine;
using System.IO;
using Infinity.Engine.Services;
using System.Windows.Controls;

namespace Infinity.Roulette.ViewModels;

public class SearchResultsViewModel : ViewModelBase
{
    private readonly ITableService _tables;
    private readonly IConcurrentSearchService _searches;
    private readonly IEngineService _engineService;
    public CancellationTokenSource cancelSource;
    public CancellationToken cancelToken
    {
        get => cancelSource.Token;
    }
    private readonly Func<int, int, bool> WinsLimitReached = (val, valLimit) => val >= valLimit;
    public SearchResultsViewModel(ITableService tables, IConcurrentSearchService searches, IEngineService engineService)
    {
        _tables = tables;
        _searches = searches;
        _engineService = engineService;

        IsLoadingEvent = false;
        Spinning = false;
        cancelSource = new();
        LoadResults();
    }
    private bool _isLoadingEvent { get; set; }
    public bool IsLoadingEvent
    {
        get => _isLoadingEvent;
        set
        {
            _isLoadingEvent = value;
            OnPropertyChanged(nameof(IsLoadingEvent));
        }
    }
    private void LoadResults()
    {
        LoadSelectedSpinfileCountDefaults();
        IsLoadingEvent = true;
        LoadedResults = [.. _searches.GetSpinResults().Where(t => t.ExactMatch && t.DoneSpinning).OrderByDescending(t => t.Rows).OrderBy(t => t.Order)];
        OpenedResults = [];
    }
    private double _GridSize { get; set; }
    public double GridSize
    {
        get => _GridSize;
        set
        {
            if (_GridSize != value)
                _GridSize = value;
            OnPropertyChanged(nameof(GridSize));
        }
    }
    private List<Table> _loadedResults { get; set; } = default!;
    public List<Table> LoadedResults
    {
        get => _loadedResults;
        set
        {
            if (_loadedResults != value)
                _loadedResults = value;
            SearchResultsCount = LoadedResults is not null ? LoadedResults.Count : 0;
            OnPropertyChanged(nameof(LoadedResults));
        }
    }
    private List<Table> _openedResults { get; set; } = default!;
    public List<Table> OpenedResults
    {
        get => _openedResults;
        set
        {
            if (_openedResults != value)
                _openedResults = value;
            OnPropertyChanged(nameof(OpenedResults));
        }
    }
    private int _SearchResultsCount { get; set; }
    public int SearchResultsCount
    {
        get => _SearchResultsCount;
        set
        {
            if (_SearchResultsCount != value)
                _SearchResultsCount = value;
            OnPropertyChanged(nameof(SearchResultsCount));
        }
    }
    private List<int> _Spinfile { get; set; } = default!;
    public List<int> Spinfile
    {
        get => _Spinfile;
        set
        {
            if (_Spinfile != value)
                _Spinfile = value;
            OnPropertyChanged(nameof(Spinfile));
        }
    }
    private bool _Spinning { get; set; }
    public bool Spinning
    {
        get => _Spinning;
        set
        {
            if (_Spinning != value)
                _Spinning = value;
            OnPropertyChanged(nameof(Spinning));
        }
    }
    private double _SpinProgress { get; set; }
    public double SpinProgress
    {
        get => _SpinProgress;
        set
        {
            if (_SpinProgress != value)
                _SpinProgress = value;
            if (_SpinProgress >= 100.0)
                Spinning = false;
            OnPropertyChanged(nameof(SpinProgress));
        }
    }
    public void AddToOpenResults(Table openedTable) => OpenedResults.Add(openedTable);
    public void LoadSpinfile(string filename)
    {
        Spinfile = [];
        using FileStream fileStream = File.Open(filename, FileMode.Open);
        using StreamReader streamReader = new(fileStream);
        while (!streamReader.EndOfStream)
        {
            string str = streamReader.ReadLine()!;
            if (!string.IsNullOrEmpty(str))
            {
                foreach (string s in str.Split('\t'))
                {
                    if (int.TryParse(s, out int result) && result > 0)
                        Spinfile.Add(result);
                }
            }
        }

        FileLoaded = true;
    }
    private bool _fileLoaded { get; set; }
    public bool FileLoaded
    {
        get => _fileLoaded;
        set
        {
            _fileLoaded = value;
            OnPropertyChanged(nameof(FileLoaded));
        }
    }
    public bool FileNotLoaded => !FileLoaded;
    private bool _stopping { get; set; }
    public bool Stopping
    {
        get => _stopping;
        set
        {
            _stopping = value;
            OnPropertyChanged(nameof(Stopping));
        }
    }
    private List<Table> _SpinFileTables { get; set; } = default!;
    public List<Table> SpinFileTables
    {
        get => _SpinFileTables;
        set
        {
            if (_SpinFileTables != value)
                _SpinFileTables = value;
            OnPropertyChanged(nameof(SpinFileTables));
        }
    }
    public async Task<CancellationToken> GetNewCancellationToken()
    {
        cancelSource = new CancellationTokenSource();
        return await Task.Run(() => cancelSource.Token);
    }
    public async Task PrepareSpinStartAsync(List<Table> spinfileTables)
    {
        SpinFileTables = await PrepareSpinfileTablesAsync(spinfileTables).ConfigureAwait(false);
        Spinning = true;
        await Task.Run(() =>
        {
            cancelSource = new();
            SpinProgress = 0.0;
            _tables.NewPlaySearch();
            _tables.ResetCounters();
            _tables.SetTotalCalculatedSpins(SpinFileTables.Count * Spinfile.Count);
            _searches.NewSpinSearch();
        }).ConfigureAwait(false);
    }
    private async Task<List<Table>> PrepareSpinfileTablesAsync(List<Table> spinfileTables)
    {
        return await Task.Run(() =>
        {
            foreach (var table in spinfileTables)
            {
                table.RunSpinfile = false;
                table.DoneSpinning = false;
            }
            return spinfileTables;
        });
    }
    public async Task PlaySpinfileTablesAsync(NewSearchResults resultsWindow, CancellationToken ct)
    {
        if (SpinFileTables.Count == 0) return;

        try
        {
            await Task.WhenAll(BuildSpinfileTableSpins(ct))
            .ContinueWith(t =>
            {
                if (t.IsCompleted) t.Dispose();

                SpinProgress = 100.0;
            }, CancellationToken.None)
            .ConfigureAwait(false);
        }
        catch (TaskCanceledException tce)
        {
            var tsk = tce.Task;
            if (tsk!.IsCompleted) tsk.Dispose();
        }
        finally
        {
            if (ct.IsCancellationRequested)
            {
                await FinalizeCancellationSpinAsync().ConfigureAwait(false);
                await Task.Run(_searches.MarkAllAsDone, CancellationToken.None).ConfigureAwait(false);
                Stopping = false;
            }
            cancelSource.Dispose();
        }

        await Task.Run(() =>
        {
            LoadResults();
            resultsWindow.ReloadGrid();
            Spinning = false;
        }, CancellationToken.None);
    }
    private Task[] BuildSpinfileTableSpins(CancellationToken ct)
    {
        List<Task> tablesToPlay = [];
        foreach (var spintable in SpinFileTables)
        {
            if (ct.IsCancellationRequested) break;
            tablesToPlay.Add(PlayTableGameSpinAsync(spintable, ct));
        }
        return [.. tablesToPlay];
    }
    private async Task<Table> PlayTableGameSpinAsync(Table table, CancellationToken ct)
    {
        if (Spinfile.Count == 0) return table;
        await Task.Run(() =>
        {
            for (int i = 0; i < Spinfile.Count; i++)
            {
                var tablePlayed = CaptureTableGameSpin(table, Spinfile.ToArray()[i], table.Spins > 0 ? table.Spins + i + 1 : i + 1, table.Spins > 0 ? table.Spins + Spinfile.Count : Spinfile.Count, ct);
                if (ct.IsCancellationRequested) break;
            }
        }, ct).ConfigureAwait(false);
        return table;
    }
    private Table CaptureTableGameSpin(Table table, int spinfileNumber, int currentSpin, int totalSpins, CancellationToken ct)
    {
        if (!ct.IsCancellationRequested)
        {
            if (table.RunSpinfile) table.RunSpinfile = false;
            if (table.DoneSpinning)
            {
                _tables.AddDoneSpins(totalSpins - table.Spins, table.TableId, table.Autoplay);
                table.DoneSpinning = true;
                SpinProgress = _tables.GetCurrentPercentage();
                return table;
            }
            table.Game.CaptureSpin(spinfileNumber, 1);

            table.WinsMatch = CheckWinsLimit(table);
            _tables.AddOverallSpin();
            SpinProgress = _tables.GetCurrentPercentage();

            if (currentSpin == totalSpins)
                table.DoneSpinning = true;

            _searches.AddSpinResult(table);

            return table;
        }
        return table;
    }
    private async Task FinalizeCancellationSpinAsync()
    {
        await Task.Delay(200);
        if (SpinProgress == 100.0)
            return;
        SpinProgress = 100.0;
    }
    public void StartSpinfileSpins(IEnumerable<Table> tables, SearchResults resultsWindow)
    {
        SpinProgress = 0.0;
        SpinFileTables = tables.ToList();
        _tables.NewPlaySearch();
        _tables.ResetCounters();
        _searches.NewSpinSearch();
        Spinning = true;
        _tables.SetTotalCalculatedSpins(tables.Count() * Spinfile.Count());
        RunSpinfileSpins(tables, resultsWindow, cancelSource.Token);
    }
    public void StartNewSpinfileSpins(IEnumerable<Table> tables, NewSearchResults resultsWindow)
    {
        FileLoaded = false;
        SpinProgress = 0.0;
        SpinFileTables = tables.ToList();
        _tables.NewPlaySearch();
        _tables.ResetCounters();
        _searches.NewSpinSearch();
        Spinning = true;
        _tables.SetTotalCalculatedSpins(tables.Count() * Spinfile.Count());
        RunNewSpinfileSpins(tables, resultsWindow, cancelSource.Token);
    }
    public async void RunSpinfileSpins(IEnumerable<Table> tables, SearchResults resultsWindow, CancellationToken token)
    {
        List<Table> list = tables.ToList();
        List<Task> tasks = [];
        for (int idx = 0; idx < list.Count; ++idx)
        {
            tasks.Add(SpinfileTableSpinTask(idx, token));
            if (token.IsCancellationRequested)
                break;
        }
        await Task.WhenAll(tasks);

        lock (tasks)
        {
            int num1 = tasks.Count(t => t.IsCanceled);
            int num2 = tasks.Count(t => t.IsCompleted);
            if (num1 <= 0 && num2 != tasks.Count)
            {
                tasks = null;
            }
            else
            {
                FinalizeCancellationSpin(resultsWindow);
                tasks = null;
            }
        }
    }
    public async void RunNewSpinfileSpins(IEnumerable<Table> tables, NewSearchResults resultsWindow, CancellationToken token)
    {
        List<Table> list = tables.ToList();
        List<Task> tasks = new();
        for (int idx = 0; idx < list.Count; ++idx)
        {
            tasks.Add(SpinfileTableSpinTask(idx, token));
            if (token.IsCancellationRequested)
                break;
        }
        await Task.WhenAll(tasks);
        //await _engineService.CompileTableSpinsIfnoAsync();

        lock (tasks)
        {
            int num1 = tasks.Count(t => t.IsCanceled);
            int num2 = tasks.Count(t => t.IsCompleted);
            if (num1 <= 0 && num2 != tasks.Count)
            {
                tasks = null;
            }
            else
            {
                FinalizeNewCancellationSpin(resultsWindow);
                tasks = null;
            }
        }
    }
    private Task SpinfileTableSpinTask(int idx, CancellationToken token) => Task.Run(async () => await RunSpinfileTableAutoplays(idx, token));
    private async Task<Task> RunSpinfileTableAutoplays(int idx, CancellationToken token)
    {
        bool cancelled = false;
        for (int autoplay = 1; autoplay <= 1; ++autoplay)
        {
            ProcessSpinfileTableAutoplayWrapper(idx, autoplay, token);
            if (token.IsCancellationRequested)
            {
                int num = await Task.Run(() => cancelled = true) ? 1 : 0;
                break;
            }
        }
        return !cancelled ? Task.CompletedTask : Task.FromCanceled(token);
    }
    public async void ProcessSpinfileTableAutoplayWrapper(int idx, int autoplay, CancellationToken token)
    {
        Task task = await ProcessSpinfileTableAutoplayTask(idx, autoplay, token);
    }
    public async Task<Task> ProcessSpinfileTableAutoplayTask(int idx, int autoplay, CancellationToken token)
    {
        bool cancelled = false;
        lock (_tables)
        {
            lock (SpinFileTables)
            {
                lock (Spinfile)
                {
                    Table table = _tables.Get(SpinFileTables[idx].TableId, autoplay)!;
                    if (table == null)
                    {
                        table = new Table(_engineService)
                        {
                            TableId = SpinFileTables[idx].TableId,
                            Autoplay = SpinFileTables[idx].Autoplay,
                            R1Wlimit = SpinFileTables[idx].R1Wlimit,
                            TWlimit = SpinFileTables[idx].TWlimit,
                            WinsMatch = SpinFileTables[idx].WinsMatch
                        };
                        table.SetTableId(SpinFileTables[idx].UniqueTableId);
                        table.SetGame(SpinFileTables[idx].Game);
                        _tables.AddTable(table);
                    }
                    if (!table.ExactMatch)
                    {
                        if (!table.DoneSpinning)
                        {
                            if (Spinfile.Count() > 0)
                            {
                                //_engineService.SetGameBoardSpinType(table.UniqueTableId, table.Game.RouletteGameId, (int)GameType.Spinfile);
                                ProcessSpinfileTableAutoplaySpin(table, Spinfile.Count(), token);
                            }
                        }
                    }
                }
            }
        }
        int num = await Task.Run(() => cancelled = token.IsCancellationRequested) ? 1 : 0;
        return !cancelled ? Task.CompletedTask : Task.FromCanceled(token);
    }
    public void ProcessSpinfileTableAutoplaySpin(Table spinTable, int totalSpins, CancellationToken token)
    {
        lock (_tables)
        {
            lock (spinTable)
            {
                lock (spinTable.Game)
                {
                    lock (Spinfile)
                    {
                        lock (_searches)
                        {
                            for (int index = 1; index <= totalSpins; ++index)
                            {
                                if (spinTable.DoneSpinning)
                                {
                                    _tables.AddDoneSpins(totalSpins - index, spinTable.TableId, spinTable.Autoplay);
                                    SpinProgress = _tables.GetCurrentPercentage();
                                    break;
                                }
                                spinTable.Game.AddSpinTypeHistory((int)GameType.Spinfile);
                                var currSpinNo = spinTable.Game.CurrentSpinNo + 1;
                                spinTable.Game.UpdateCurrentSpin(currSpinNo);
                                spinTable.Game.CaptureSpin(Spinfile.ToArray()[index - 1], 1);
                                _tables.AddOverallSpin();
                                SpinProgress = _tables.GetCurrentPercentage();
                                spinTable.WinsMatch = CheckWinsLimit(spinTable);
                                if (index == totalSpins)
                                    spinTable.DoneSpinning = true;
                                _searches.AddSpinResult(spinTable);
                                if (token.IsCancellationRequested)
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
    public async void StopSpins() => await Task.Run(StopTableRuns);
    private void StopTableRuns()
    {
        if (cancelSource != null)
            cancelSource.Cancel();
        SpinProgress = 100.0;
    }
    private void FinalizeCancellationSpin(SearchResults resultsWindow)
    {
        if (SpinProgress != 100.0)
            SpinProgress = 100.0;
        LoadResults();
        resultsWindow.ReloadGrid();
    }
    private void FinalizeNewCancellationSpin(NewSearchResults resultsWindow)
    {
        if (SpinProgress != 100.0)
            SpinProgress = 100.0;
        LoadResults();
        resultsWindow.ReloadGrid();
    }
    private bool CheckWinsLimit(Table table)
    {
        lock (table)
            return (new bool[2]
            {
          CheckR1WLimit(table),
          CheckTWLimit(table)
            }).Count(wl => wl) > 0;
    }
    private bool CheckR1WLimit(Table table)
    {
        lock (table)
        {
            int? r1Wlimit = table.R1Wlimit;
            if (!r1Wlimit.HasValue)
                return false;
            Func<int, int, bool> winsLimitReached = WinsLimitReached;
            int firstRowWin = table.FirstRowWin;
            r1Wlimit = table.R1Wlimit;
            int num = r1Wlimit!.Value;
            return winsLimitReached(firstRowWin, num);
        }
    }
    private bool CheckTWLimit(Table table)
    {
        lock (table)
        {
            int? twlimit = table.TWlimit;
            if (!twlimit.HasValue)
                return false;
            Func<int, int, bool> winsLimitReached = WinsLimitReached;
            int highestColumnWin = table.HighestColumnWin;
            twlimit = table.TWlimit;
            int num = twlimit!.Value;
            return winsLimitReached(highestColumnWin, num);
        }
    }
    public bool IsNotPlaying => !Spinning;

    private List<ComboBoxItem> _spinfileCountOptions { get; set; } = [];
    public List<ComboBoxItem> SpinfileCountOptions
    {
        get => _spinfileCountOptions;
        set
        {
            if (_spinfileCountOptions != value) _spinfileCountOptions = value;
            OnPropertyChanged(nameof(SpinfileCountOptions));
        }
    }

    private int _selectedSpinfileCount { get; set; } = -2;
    public int SelectedSpinfileCount
    {
        get => _selectedSpinfileCount;
        set
        {
            if (_selectedSpinfileCount != value) _selectedSpinfileCount = value;
            OnPropertyChanged(nameof(SelectedSpinfileCount));
        }
    }

    public void LoadSelectedSpinfileCountDefaults()
    {
        SelectedSpinfileCount = -2;     
    }
}
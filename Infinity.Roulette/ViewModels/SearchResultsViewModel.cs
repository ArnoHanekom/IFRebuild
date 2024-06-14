// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.SearchResultsViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using Infinity.Engine;
using System.IO;
using Infinity.Engine.Services;

#nullable enable
namespace Infinity.Roulette.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private readonly ITableService _tables;
        private readonly ISearchService _searches;
        private readonly IEngineService _engineService;
        
        private CancellationTokenSource cancellationToken;
        private Func<int, int, bool> WinsLimitReached = (val, valLimit) => val >= valLimit;

        public SearchResultsViewModel(ITableService tables, ISearchService searches, IEngineService engineService)
        {            
            _tables = tables;
            _searches = searches;
            _engineService = engineService;
            
            Spinning = false;
            cancellationToken = new CancellationTokenSource();
            LoadResults();
        }

        private void LoadResults()
        {
            LoadedResults = _searches.GetSpinResults();
            OpenedResults = new List<Table>();
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
            get => _loadedResults.OrderBy(t => t.Order).ToList();
            set
            {
                if (_loadedResults != value)
                    _loadedResults = value;
                SearchResultsCount = LoadedResults != null ? LoadedResults.Count() : 0;
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

        private bool _ShowRunBtn { get; set; }

        public bool ShowRunBtn
        {
            get => _ShowRunBtn;
            set
            {
                if (_ShowRunBtn != value)
                    _ShowRunBtn = value;
                OnPropertyChanged(nameof(ShowRunBtn));
            }
        }

        private bool _ShowStopBtn { get; set; }

        public bool ShowStopBtn
        {
            get => _ShowStopBtn;
            set
            {
                if (_ShowStopBtn != value)
                    _ShowStopBtn = value;
                OnPropertyChanged(nameof(ShowStopBtn));
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
                ShowRunBtn = !_Spinning;
                ShowStopBtn = _Spinning;
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

        private int _SelectedAutoplayValue { get; set; }

        public int SelectedAutoplayValue
        {
            get => _SelectedAutoplayValue;
            set
            {
                if (_SelectedAutoplayValue != value)
                    _SelectedAutoplayValue = value;
                OnPropertyChanged(nameof(SelectedAutoplayValue));
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
            cancellationToken = new CancellationTokenSource();
            return await Task.Run(() => cancellationToken.Token);
        }

        public void StartSpinfileSpins(IEnumerable<Table> tables, SearchResults resultsWindow)
        {
            SpinProgress = 0.0;
            SelectedAutoplayValue = 1;
            SpinFileTables = tables.ToList();
            _tables.NewPlaySearch();
            _tables.ResetCounters();
            _searches.NewSpinSearch();
            Spinning = true;
            _tables.SetTotalCalculatedSpins(tables.Count() * Spinfile.Count());
            RunSpinfileSpins(tables, cancellationToken.Token, resultsWindow);
        }

        public async void RunSpinfileSpins(
          IEnumerable<Table> tables,
          CancellationToken token,
          SearchResults resultsWindow)
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
                    FinalizeCancellationSpin(resultsWindow);
                    tasks = null;
                }
            }
        }

        private Task SpinfileTableSpinTask(int idx, CancellationToken token) => Task.Run(async () => await RunSpinfileTableAutoplays(idx, token));

        private async Task<Task> RunSpinfileTableAutoplays(int idx, CancellationToken token)
        {
            bool cancelled = false;
            for (int autoplay = 1; autoplay <= SelectedAutoplayValue; ++autoplay)
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

        public async void ProcessSpinfileTableAutoplayWrapper(
          int idx,
          int autoplay,
          CancellationToken token)
        {
            Task task = await ProcessSpinfileTableAutoplayTask(idx, autoplay, token);
        }

        public async Task<Task> ProcessSpinfileTableAutoplayTask(
          int idx,
          int autoplay,
          CancellationToken token)
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

        public void ProcessSpinfileTableAutoplaySpin(
          Table spinTable,
          int totalSpins,
          CancellationToken token)
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
                                        _tables.AddDoneSpins(totalSpins - index);
                                        SpinProgress = _tables.GetCurrentPercentage();
                                        break;
                                    }
                                    spinTable.Game.AddSpinTypeHistory((int)GameType.Spinfile);
                                    var currSpinNo = spinTable.Game.CurrentSpinNo + 1;
                                    spinTable.Game.UpdateCurrentSpin(currSpinNo);
                                    spinTable.Game.CaptureSpin(Spinfile[index - 1], 1);
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

        public async void StopSpins() => await Task.Run(() => StopTableRuns());

        private void StopTableRuns()
        {
            if (cancellationToken != null)
                cancellationToken.Cancel();
            SpinProgress = 100.0;
        }

        private void FinalizeCancellationSpin(SearchResults resultsWindow)
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
    }
}

using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Engine;
using Infinity.Engine.Services;
using Infinity.Roulette.Statics;
using Infinity.Services.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infinity.Roulette.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ITableService _tables;
    private readonly INumberGenerator _numberGenerator;
    private readonly ISettingService _settingService;
    private readonly IGameTypeService _gameTypeService;

    //private readonly ISearchService _searches;
    private readonly IConcurrentSearchService _searches;

    private readonly ICountColorService _countColorService;
    private readonly IEngineService _engineService;
    //private readonly IOddWinService _oddWinService;
    public CancellationTokenSource Cts;
    private Func<int, int, bool> LimitReached = (val, valLimit) => val == valLimit;
    private Func<int, int, bool> WinsLimitReached = (val, valLimit) => val >= valLimit;

    public CancellationTokenSource cancelSource = new();
    public CancellationToken cancelToken
    {
        get => cancelSource.Token;
    }

    private int _ExactMatchFontSize { get; set; }
    private Setting _setting { get; set; } = default!;
    public Setting Setting
    {
        get => _setting;
        set
        {
            if (_setting != value)
                _setting = value;
            OnPropertyChanged(nameof(Setting));
        }
    }
    public int ExactMatchFontSize
    {
        get => _ExactMatchFontSize;
        set
        {
            if (_ExactMatchFontSize != value)
                _ExactMatchFontSize = value;
            OnPropertyChanged(nameof(ExactMatchFontSize));
        }
    }
    private int _ExactMatchCount { get; set; }

    public int ExactMatchCount
    {
        get => _ExactMatchCount;
        set
        {
            if (_ExactMatchCount != value)
                _ExactMatchCount = value;
            OnPropertyChanged(nameof(ExactMatchCount));
        }
    }

    private int _EstimatedSpinsTotal { get; set; }

    public int EstimatedSpinsTotal
    {
        get => _EstimatedSpinsTotal;
        set
        {
            if (_EstimatedSpinsTotal != value)
                _EstimatedSpinsTotal = value;
            OnPropertyChanged(nameof(EstimatedSpinsTotal));
        }
    }

    private int _R1WMatchFontSize { get; set; }

    public int R1WMatchFontSize
    {
        get => _R1WMatchFontSize;
        set
        {
            if (_R1WMatchFontSize != value)
                _R1WMatchFontSize = value;
            OnPropertyChanged(nameof(R1WMatchFontSize));
        }
    }

    private int _R1WMatchCount { get; set; }

    public int R1WMatchCount
    {
        get => _R1WMatchCount;
        set
        {
            if (_R1WMatchCount != value)
                _R1WMatchCount = value;
            OnPropertyChanged(nameof(R1WMatchCount));
        }
    }

    private int _TWMatchFontSize { get; set; }

    public int TWMatchFontSize
    {
        get => _TWMatchFontSize;
        set
        {
            if (_TWMatchFontSize != value)
                _TWMatchFontSize = value;
            OnPropertyChanged(nameof(TWMatchFontSize));
        }
    }

    private int _TWMatchCount { get; set; }

    public int TWMatchCount
    {
        get => _TWMatchCount;
        set
        {
            if (_TWMatchCount != value)
                _TWMatchCount = value;
            OnPropertyChanged(nameof(TWMatchCount));
        }
    }

    private int? _Tables { get; set; }

    public int? Tables
    {
        get => _Tables;
        set
        {
            int? tables1 = _Tables;
            int? nullable1 = value;
            if (!(tables1.GetValueOrDefault() == nullable1.GetValueOrDefault() & tables1.HasValue == nullable1.HasValue))
            {
                _Tables = value;
                int? nullable2 = _Tables;
                if (nullable2.HasValue)
                {
                    nullable2 = _Tables;
                    int tables2 = nullable2.Value;
                    nullable2 = Random;
                    int randoms;
                    if (!nullable2.HasValue)
                    {
                        randoms = 0;
                    }
                    else
                    {
                        nullable2 = Random;
                        randoms = nullable2.Value;
                    }
                    RecalcTotals(tables2, randoms);
                }
            }
            RecalcTotalResults();
            OnPropertyChanged(nameof(Tables));
        }
    }

    private int? _Random { get; set; }

    public int? Random
    {
        get => _Random;
        set
        {
            int? random = _Random;
            int? nullable1 = value;
            if (!(random.GetValueOrDefault() == nullable1.GetValueOrDefault() & random.HasValue == nullable1.HasValue))
            {
                _Random = value;
                int? nullable2 = _Random;
                if (nullable2.HasValue)
                {
                    nullable2 = Tables;
                    int tables;
                    if (!nullable2.HasValue)
                    {
                        tables = 0;
                    }
                    else
                    {
                        nullable2 = Tables;
                        tables = nullable2.Value;
                    }
                    nullable2 = _Random;
                    int randoms = nullable2.Value;
                    RecalcTotals(tables, randoms);
                }
            }
            OnPropertyChanged(nameof(Random));
        }
    }

    private int? _RowLimit { get; set; }

    public int? RowLimit
    {
        get => _RowLimit;
        set
        {
            int? rowLimit = _RowLimit;
            int? nullable = value;
            if (!(rowLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & rowLimit.HasValue == nullable.HasValue))
                _RowLimit = value;
            OnPropertyChanged(nameof(RowLimit));
        }
    }

    private int? _CountLimit { get; set; }

    public int? CountLimit
    {
        get => _CountLimit;
        set
        {
            int? countLimit = _CountLimit;
            int? nullable = value;
            if (!(countLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & countLimit.HasValue == nullable.HasValue))
                _CountLimit = value;
            OnPropertyChanged(nameof(CountLimit));
        }
    }

    private int? _GSLimit { get; set; }

    public int? GSLimit
    {
        get => _GSLimit;
        set
        {
            int? gsLimit = _GSLimit;
            int? nullable = value;
            if (!(gsLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & gsLimit.HasValue == nullable.HasValue))
                _GSLimit = value;
            OnPropertyChanged(nameof(GSLimit));
        }
    }

    private int? _R1WLimit { get; set; }

    public int? R1WLimit
    {
        get => _R1WLimit;
        set
        {
            int? r1Wlimit = _R1WLimit;
            int? nullable = value;
            if (!(r1Wlimit.GetValueOrDefault() == nullable.GetValueOrDefault() & r1Wlimit.HasValue == nullable.HasValue))
                _R1WLimit = value;
            OnPropertyChanged(nameof(R1WLimit));
        }
    }

    private int? _TWLimit { get; set; }

    public int? TWLimit
    {
        get => _TWLimit;
        set
        {
            int? twLimit = _TWLimit;
            int? nullable = value;
            if (!(twLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & twLimit.HasValue == nullable.HasValue))
                _TWLimit = value;
            OnPropertyChanged(nameof(TWLimit));
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
            OnPropertyChanged(nameof(IsNotPlaying));
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
            if (SpinProgress >= 100.0) Spinning = false;
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
            RecalcTotalResults();
            OnPropertyChanged(nameof(SelectedAutoplayValue));
        }
    }

    private int? _CalculatedTotalResults { get; set; }
    public int? CalculatedTotalResults
    {
        get => _CalculatedTotalResults;
        set
        {
            int? calculatedTotalResults = _CalculatedTotalResults;
            int? nullable = value;
            if (!(calculatedTotalResults.GetValueOrDefault() == nullable.GetValueOrDefault() & calculatedTotalResults.HasValue == nullable.HasValue))
                _CalculatedTotalResults = value;
            OnPropertyChanged(nameof(CalculatedTotalResults));
        }
    }

    private IEnumerable<ComboBoxItem> _AutoplayOptions { get; set; } = default!;

    public IEnumerable<ComboBoxItem> AutoplayOptions
    {
        get => _AutoplayOptions;
        set
        {
            if (_AutoplayOptions != value)
                _AutoplayOptions = value;
            OnPropertyChanged(nameof(AutoplayOptions));
        }
    }

    private ComboBoxItem? _SelectedAutoplay { get; set; } = default!;

    public ComboBoxItem? SelectedAutoplay
    {
        get => _SelectedAutoplay;
        set
        {
            if (_SelectedAutoplay != value)
                _SelectedAutoplay = value;
            if (_SelectedAutoplay != null)
            {
                int result;
                if (int.TryParse(_SelectedAutoplay.Content.ToString(), out result))
                    SelectedAutoplayValue = result;
            }
            else
                SelectedAutoplayValue = 1;
            OnPropertyChanged(nameof(SelectedAutoplay));
        }
    }

    private GameType _RouletteGameType { get; set; } = default!;
    public GameType RouletteGameType
    {
        get => _RouletteGameType;
        set
        {
            if (_RouletteGameType != value)
                _RouletteGameType = value;
            RecalcTotalResults();
            OnPropertyChanged(nameof(RouletteGameType));
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

    private GameSetting _gameSetting { get; set; } = default!;

    public GameSetting GameSetting
    {
        get => _gameSetting;
        set
        {
            if (_gameSetting != value)
                _gameSetting = value;
            OnPropertyChanged(nameof(GameSetting));
        }
    }

    public MainViewModel(ITableService tables, INumberGenerator numberGenerator, ISettingService settingService,
      IGameTypeService gameTypeService, IConcurrentSearchService searches, ICountColorService countColorService, IEngineService engineService)
    {
        _tables = tables;
        _searches = searches;
        _numberGenerator = numberGenerator;
        _settingService = settingService;
        _gameTypeService = gameTypeService;
        _countColorService = countColorService;
        _engineService = engineService;
        //_oddWinService = oddWinService;
        Cts = new CancellationTokenSource();
        Spinning = false;
        LoadDefaults();
    }

    private void RecalcTotalResults()
    {
        if (RouletteGameType == GameType.Autoplay)
            RecalcAutoplayTotalResults();
        else
            RecalcRandomTotalResults();
    }

    private void RecalcRandomTotalResults() => CalculatedTotalResults = new int?(Tables.HasValue ? Tables.Value : 0);

    private void RecalcAutoplayTotalResults() => CalculatedTotalResults = new int?((Tables.HasValue ? Tables.Value : 0) * SelectedAutoplayValue);

    public void ReloadSettings(Setting newSetting)
    {
        Setting = newSetting;
        LoadGameSetting(Setting.DefaultGameType);
        LoadGameTypeSettings();
    }

    private void LoadDefaults()
    {
        _tables.ResetCounters();
        SetFontSizes();
        CheckAndLoadAutoplaySettings();
        GetLatestSetting();
        LoadGameSetting(Setting.DefaultGameType);
        RouletteGameType = Setting.DefaultGameType;
        if (RouletteGameType != GameType.None)
            LoadGameTypeSettings();
        Spinning = false;
        SpinProgress = 0.0;
        ExactMatchCount = 0;
        TotalCalculatedSpins = 0;
    }
    public GameType SelectedGameType => RouletteGameType;

    private void SetFontSizes()
    {
        ExactMatchFontSize = 14;
        R1WMatchFontSize = 14;
        TWMatchFontSize = 14;
    }


    private void GetLatestSetting() => Setting = _settingService.Get();

    private void OverwriteGameTypeDefaults()
    {
        Tables = GameSetting.PlayTables;
        Random = GameSetting.RandomNumbers;
        RowLimit = GameSetting.RowLimit;
        CountLimit = GameSetting.CountLimit;
        GSLimit = GameSetting.GSLimit;
        R1WLimit = GameSetting.R1WLimit;
        TWLimit = GameSetting.TWLimit;
        CheckAndLoadAutoplaySettings();
    }

    private void CheckAndLoadAutoplaySettings()
    {
        var autoplaySelection = Autoplays.Selection;
        AutoplayOptions = [];
        int autoplaySetting = (GameSetting?.AutoplayNumber) ?? 10;
        List<ComboBoxItem> items = [];
        for (int i = 0; i < autoplaySelection.Count; i++)
        {
            if (autoplaySelection[i] == autoplaySetting)
            {
                SelectedAutoplay = new ComboBoxItem()
                {
                    Content = autoplaySelection[i].ToString(),
                    FontFamily = new FontFamily("Century Gothic"),
                    IsSelected = true
                };
                items.Add(SelectedAutoplay);
            }
            else
            {
                items.Add(new()
                {
                    Content = autoplaySelection[i].ToString(),
                    FontFamily = new FontFamily("Century Gothic")
                });
            }
        }
        AutoplayOptions = items.AsEnumerable();
    }

    public void LoadGameTypeSettings()
    {
        SetFontSizes();
        GetLatestSetting();
        LoadGameSetting(RouletteGameType);
        if (GameSetting == null)
            return;
        OverwriteGameTypeDefaults();
    }

    public void LoadGameSetting(GameType typeToLoad) => GameSetting = Setting.GameSettings.FirstOrDefault(gs => gs.Type == _gameTypeService.Get(typeToLoad.ToString()))!;

    public void ProcessReset() => LoadDefaults();

    public void ProcessFullReset()
    {
        _clearDefaultSettings();
        LoadDefaults();
    }

    private void _clearDefaultSettings()
    {
        FileInfo fileInfo = new("settings.json");
        if (fileInfo.Exists)
            fileInfo.Delete();
        Setting = new();
    }

    private void RecalcTotals(int tables, int randoms)
    {
        int num;
        if (RouletteGameType == GameType.Autoplay)
        {
            int result = 1;
            if (SelectedAutoplay != null)
                int.TryParse(SelectedAutoplay.Content.ToString(), out result);
            num = tables * randoms * result;
        }
        else
            num = tables * randoms;
        EstimatedSpinsTotal = num;
    }
    public bool IsNotPlaying => !Spinning;

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
    public void SetOtherCountStyle()
    {
        Style otherStyle = (Style)Application.Current.FindResource("BetWindowLabelTextBlockOtherSpinCount");
        _countColorService.SetOtherStyle(otherStyle);
    }
    public async Task PlaySpinsAsync(CancellationToken ct)
    {        
        await StartUpSpinsAsync(ct).ConfigureAwait(false);
    }
    public async Task PreparePlayStartAsync()
    {
        await Task.Run(() =>
        {
            cancelSource = new();
            SpinProgress = 0.0;
            ExactMatchCount = 0;
            R1WMatchCount = 0;
            TWMatchCount = 0;
            if (RouletteGameType == GameType.Random)
                SelectedAutoplayValue = 1;
            Spinning = true;
            var autoplayVal = RouletteGameType == GameType.Random ? 1 : (SelectedAutoplay == null ? 1 : SelectedAutoplayValue);
            _tables.SetTotalCalculatedSpins(Tables!.Value * Random!.Value * autoplayVal);
            TotalCalculatedSpins = _tables.GetTotalCalculatedSpins();
        }).ConfigureAwait(false);
    }

    private async Task StartUpSpinsAsync(CancellationToken ct)
    {
        if (!Tables.HasValue && !Random.HasValue) return;
        _searches.NewSpinSearch();
        _tables.NewPlaySearch();
        try
        {
            await Task.WhenAll(BuildTableSpins(ct))
                .ContinueWith(t => {
                    //var stillRunning = _searches.GetSpinResults().Where(r => !r.DoneSpinning);
                    //if (stillRunning.Any(r => !r.DoneSpinning)) DebugPrinting.Add($"{stillRunning.Count()} tables still running");
                    if (t.IsCompleted) t.Dispose();
                    Spinning = false;
                    SpinProgress = 100.0;
                }, CancellationToken.None)
                //.ContinueWith(t => {                    
                //    Task.Run(async ()=>
                //    {
                //        await Task.Run(() =>
                //        {
                //            foreach (var logLine in DebugPrinting)
                //            {
                //                Debug.WriteLine($"\t'{logLine}'");
                //            }
                //        });
                //    });
                //}, ct)
                .ConfigureAwait(false);
        }
        catch (TaskCanceledException tce)
        {
            var tsk = tce.Task;
            if (tsk!.IsCompleted) tsk.Dispose();
            //maybe keep track of cancelled tasks?
            //tce.Task
        }
        finally
        {
            if (ct.IsCancellationRequested)
            {
                await FinalizeCancellationSpinAsync().ConfigureAwait(false);
                await Task.Run(() =>
                {
                    _searches.MarkAllAsDone();
                    MatchCountExact = _searches
                        .GetSpinResults()
                        .Count(t => t.Matched == 1 && t.DoneSpinning);
                }, CancellationToken.None).ConfigureAwait(false);

                Stopping = false;
            }            
            cancelSource.Dispose();
        }
    }
    private Task[] BuildTableSpins(CancellationToken ct)
    {    
        List<Task> tablesToPlay = [];
        for (int t = 0; t < Tables!.Value; t++)
        {
            if (ct.IsCancellationRequested) break;
            tablesToPlay.Add(PlayTableAutoplaysAsync(t + 1, ct));
        }
        return [.. tablesToPlay];
    }

    private async Task PlayTableAutoplaysAsync(int tableId, CancellationToken ct)
    {
        for (int ap = 0; ap < SelectedAutoplayValue; ap++)
        {            
            await PlayTableGameAutoplayAsync(tableId, ap, ct).ConfigureAwait(false);
            if (ct.IsCancellationRequested) break;
        }
    }

    private async Task<Table> PlayTableGameAutoplayAsync(int tableId, int currentAp, CancellationToken ct)
    {        
        Table tableForPlay = TableForPlay(tableId, currentAp);
        if (ct.IsCancellationRequested) return tableForPlay;        
        return await PlayTableGameSpinAsync(tableForPlay, ct).ConfigureAwait(false);
    }
    private async Task<Table> PlayTableGameSpinAsync(Table table, CancellationToken ct)
    {
        if (!Random.HasValue) return table; 
        await Task.Run(() =>
        {
            for (int i = 0; i < Random.Value; i++)
            {   
                var tablePlayed = CaptureTableGameSpin(table, i + 1, Random.Value, ct);
                if ((i + 1) == Random.Value || tablePlayed.ExactMatch || tablePlayed.WinsMatch)
                    _searches.AddSpinResult(tablePlayed);
                lock (_searches)
                {
                    MatchCountExact = _searches.GetSpinResults().Where(t => t.Matched == 1 && t.DoneSpinning).Count();
                }
                if (ct.IsCancellationRequested) break;                
            }
        }, ct).ConfigureAwait(false);
        return table;
    }

    private ConcurrentBag<string> DebugPrinting { get; set; } = [];

    private Table CaptureTableGameSpin(Table gameTable, int currentSpin, int totalSpins, CancellationToken ct)
    {
        if (!ct.IsCancellationRequested)
        {
            if (gameTable.DoneSpinning)
            {
                _tables.AddDoneSpins(totalSpins - gameTable.Spins);
                gameTable.DoneSpinning = true;
                SpinProgress = _tables.GetCurrentPercentage();
                return gameTable;
            }
            gameTable.Game.CaptureSpin(_numberGenerator.NextRandomNumber(), 0);
            gameTable.ExactMatch = CheckLimit(gameTable);
            gameTable.WinsMatch = CheckWinsLimit(gameTable); 
            _tables.AddOverallSpin();
            SpinProgress = _tables.GetCurrentPercentage();

            if (currentSpin == totalSpins || gameTable.ExactMatch)
                gameTable.DoneSpinning = true;
            
            return gameTable;
        }
        return gameTable;
    }

    private int _matchCountExact { get; set; }
    public int MatchCountExact
    {
        get => _matchCountExact;
        set
        {
            _matchCountExact = value;
            OnPropertyChanged(nameof(MatchCountExact));
        }
    }

    private int _totalCalculatedSpins { get; set; }
    public int TotalCalculatedSpins
    {
        get => _totalCalculatedSpins;
        set
        {
            _totalCalculatedSpins = value;
            OnPropertyChanged(nameof(TotalCalculatedSpins));
        }
    }

    private Table TableForPlay(int id, int autoplay)
    {
        var t = _getPlayTable(_tables, id, autoplay);
        if (t is null)
        {
            t = new (_engineService)
            {
                TableId = id,
                Autoplay = autoplay,
                R1Wlimit = R1WLimit,
                TWlimit = TWLimit
            };
            _tables.AddTable(t);
        }
        return t;
    }

    public Setting CleanDashboardSetting()
    {
        _settingService.New();
        return _settingService.Get();
    }

    private async Task FinalizeCancellationSpinAsync()
    {
        await Task.Delay(200);
        if (SpinProgress == 100.0)
            return;
        SpinProgress = 100.0;
    }

    private void FinalizeCancellationSpin()
    {
        Task.Delay(200);
        if (SpinProgress == 100.0)
            return;
        SpinProgress = 100.0;
    }

    private Func<ITableService, int, int, Table?> _getPlayTable = (tableService, id, ap) =>
    {
        lock (tableService)
        {
            return tableService.Get(id, ap);
        }
    };

    private bool CheckLimit(Table table)
    {
        lock (table)
        {
            int? nullable = CountLimit;
            int num1 = nullable.HasValue ? 1 : 0;
            nullable = RowLimit;
            bool hasValue1 = nullable.HasValue;
            nullable = GSLimit;
            bool hasValue2 = nullable.HasValue;
            nullable = CountLimit;
            int num2;
            if (!nullable.HasValue)
            {
                num2 = 0;
            }
            else
            {
                nullable = CountLimit;
                num2 = nullable.Value;
            }
            int num3 = num2;
            nullable = RowLimit;
            int num4;
            if (!nullable.HasValue)
            {
                num4 = 0;
            }
            else
            {
                nullable = RowLimit;
                num4 = nullable.Value;
            }
            int num5 = num4;
            nullable = GSLimit;
            int num6;
            if (!nullable.HasValue)
            {
                num6 = 0;
            }
            else
            {
                nullable = GSLimit;
                num6 = nullable.Value;
            }
            int num7 = num6;
            int counts = table.Game.GetCounts();
            int rows = table.Game.GetRows();
            int gs = table.Game.GetGS();
            bool flag = false;
            if ((num1 & (hasValue1 ? 1 : 0) & (hasValue2 ? 1 : 0)) != 0)
                flag = LimitReached(counts, num3) && LimitReached(rows, num5) && LimitReached(gs, num7);
            if ((num1 & (hasValue1 ? 1 : 0)) != 0 && !hasValue2)
                flag = LimitReached(counts, num3) && LimitReached(rows, num5);
            if (num1 != 0 && !hasValue1 && !hasValue2)
                flag = LimitReached(counts, num3);
            if (num1 == 0 & hasValue1 && !hasValue2)
                flag = LimitReached(rows, num5);
            if (((num1 != 0 ? 0 : (!hasValue1 ? 1 : 0)) & (hasValue2 ? 1 : 0)) != 0)
                flag = LimitReached(gs, num7);
            if (num1 == 0 & hasValue1 & hasValue2)
                flag = LimitReached(rows, num5) && LimitReached(gs, num7);
            return flag;
        }
    }
    private bool CheckWinsLimit(Table table)
    {
        lock (table)
            return (new bool[2]
            {
                    CheckR1WLimit(table),
                    CheckTWLimit(table)
            }).Any(wl => wl);
    }
    private bool CheckR1WLimit(Table table)
    {
        lock (table)
        {
            int? r1Wlimit = R1WLimit;
            if (!r1Wlimit.HasValue)
                return false;
            Func<int, int, bool> winsLimitReached = WinsLimitReached;
            int firstRowWin = table.FirstRowWin;
            r1Wlimit = R1WLimit;
            int num = r1Wlimit.Value;
            return winsLimitReached(firstRowWin, num); 
        }
    }
    private bool CheckTWLimit(Table table)
    {
        lock (table)
        {
            int? twLimit = TWLimit;
            if (!twLimit.HasValue)
                return false;
            Func<int, int, bool> winsLimitReached = WinsLimitReached;
            int highestColumnWin = table.HighestColumnWin;
            twLimit = TWLimit;
            int num = twLimit.Value;
            return winsLimitReached(highestColumnWin, num); 
        }
    }
    public void LoadAndShowNewTable(Window ownerWindow)
    {
        NewTable newTable = new NewTable(_engineService);
        newTable.Owner = ownerWindow;
        newTable.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        newTable.Show();
        newTable.Owner = null;
    }

    private bool _searching;
    public bool Searching
    {
        get => _searching;
        private set => _searching = value;
    }
}
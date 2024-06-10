// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.MainViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Data.Constants;
using Infinity.Data.Extensions;
using Infinity.Data.Models;
using Infinity.Engine;
using Infinity.Engine.Services;
using Infinity.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


#nullable enable
namespace Infinity.Roulette.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITableService _tables;
        private readonly INumberGenerator _numberGenerator;
        private readonly ISettingService _settingService;
        private readonly IGameTypeService _gameTypeService;
        private readonly ISearchService _searches;
        private readonly ICountColorService _countColorService;
        private readonly IEngineService _engineService;
        //private readonly IOddWinService _oddWinService;
        private CancellationTokenSource cancellationToken;
        private Func<int, int, bool> LimitReached = (val, valLimit) => val == valLimit;
        private Func<int, int, bool> WinsLimitReached = (val, valLimit) => val >= valLimit;

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
                {
                    Spinning = false;
                    WriteOddWins();
                }                    
                OnPropertyChanged(nameof(SpinProgress));
            }
        }

        private void WriteOddWins()
        {
            //var oddWins = _oddWinService.GetOddWins();
                //Debug.WriteLine("**********   ODD WINS   *************");
                //if (oddWins.Count == 0)
                //    Debug.WriteLine("No odd wins found");
                //else
                //{
                //    foreach (var oddWin in oddWins)
                //    {
                //        Debug.WriteLine($"Spin: {oddWin.SpinNumber}");
                //        if (oddWin.WinNumber is not null)
                //            Debug.WriteLine($"Win Number: { JsonConvert.SerializeObject(oddWin.WinNumber) }");
                //        Debug.WriteLine($"No Code Number: {JsonConvert.SerializeObject(oddWin.NoCodeNumber)}");
                //    }
                //}

                //Debug.WriteLine("**********     DONE     *************");
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

        private List<int> _autoplayPreList { get; set; } = default!;

        public List<int> AutoplayPrepList
        {
            get => _autoplayPreList;
            set
            {
                if (_autoplayPreList != value)
                    _autoplayPreList = value;
                OnPropertyChanged(nameof(AutoplayPrepList));
            }
        }

        public MainViewModel(ITableService tables, INumberGenerator numberGenerator, ISettingService settingService,
          IGameTypeService gameTypeService, ISearchService searches, ICountColorService countColorService, IEngineService engineService)
        {            
            _tables = tables;
            _searches = searches;
            _numberGenerator = numberGenerator;
            _settingService = settingService;
            _gameTypeService = gameTypeService;
            _countColorService = countColorService;
            _engineService = engineService;
            //_oddWinService = oddWinService;
            cancellationToken = new CancellationTokenSource();
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
            ResetCounters();
            LoadGameTypeSettings();
        }

        private void LoadDefaults()
        {
            _tables.ResetCounters();
            PrepareAutoplayOptions();
            RouletteGameType = GameType.Autoplay;
            SetFontSizes();
            ResetCounters();
            CheckAndLoadAutoplaySettings();
            GetLatestSetting();
            LoadGameSetting(Setting.DefaultGameType);
            if (RouletteGameType == Setting.DefaultGameType)
                LoadGameTypeSettings();
            RouletteGameType = Setting.DefaultGameType;
        }

        public async Task<GameType> GetSelectedGameType() => await Task.Run(() => RouletteGameType);

        private void SetFontSizes()
        {
            ExactMatchFontSize = 14;
            R1WMatchFontSize = 14;
            TWMatchFontSize = 14;
        }

        private void ResetCounters()
        {
            ExactMatchCount = 0;
            R1WMatchCount = 0;
            TWMatchCount = 0;
            SpinProgress = 0.0;
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
            AutoplayOptions = new HashSet<ComboBoxItem>();
            SelectedAutoplay = null;
            int autoplaySetting = (GameSetting?.AutoplayNumber) ?? 10;
            AutoplayOptions = AutoplayPrepList.Select(c =>
            {
                if (c == autoplaySetting)
                {
                    SelectedAutoplay = new ComboBoxItem()
                    {
                        Content = c.ToString(),
                        FontFamily = new FontFamily("Century Gothic"),
                        IsSelected = true
                    };
                    return SelectedAutoplay;
                }
                return new ComboBoxItem()
                {
                    Content = c.ToString(),
                    FontFamily = new FontFamily("Century Gothic")
                };
            });
        }

        public void LoadGameTypeSettings()
        {
            PrepareAutoplayOptions();
            SetFontSizes();
            ResetCounters();
            GetLatestSetting();
            LoadGameSetting(RouletteGameType);
            if (GameSetting == null)
                return;
            OverwriteGameTypeDefaults();
        }

        public void LoadGameSetting(GameType typeToLoad) => GameSetting = Setting.GameSettings.FirstOrDefault(gs => gs.Type == _gameTypeService.Get(typeToLoad.ToString()))!;

        private void PrepareAutoplayOptions()
        {
            AutoplayPrepList = new List<int>();
            for (int index = 10; index <= 100; index += 10)
                AutoplayPrepList.Add(index);
            for (int index = 120; index <= 200; index += 20)
                AutoplayPrepList.Add(index);
            for (int index = 250; index <= 300; index += 50)
                AutoplayPrepList.Add(index);
        }

        public void ProcessReset() => LoadDefaults();

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

        public async Task<CancellationToken> GetNewCancellationToken()
        {
            cancellationToken = new CancellationTokenSource();
            return await Task.Run(() => cancellationToken.Token);
        }

        public async void StartSpins(CancellationToken token)
        {
            SpinProgress = 0.0;
            ExactMatchCount = 0;
            R1WMatchCount = 0;
            TWMatchCount = 0;
            if (RouletteGameType == GameType.Random)
                SelectedAutoplayValue = 1;
            Spinning = true;
            _searches.NewSpinSearch();
            await Task.Run(async () => await RunTables(token));
            //await WaitTillDoneSpinning();
        }

        public async void StopSpins() => await Task.Run(() => StopTableRuns());

        private void StopTableRuns()
        {
            if (cancellationToken != null)
                cancellationToken.Cancel();
            SpinProgress = 100.0;
        }

        public void SetOtherCountStyle()
        {
            Style otherStyle = (Style)Application.Current.FindResource("BetWindowLabelTextBlockOtherSpinCount");
            _countColorService.SetOtherStyle(otherStyle);
        }

        public async Task RunTables(CancellationToken token)
        {
            _tables.NewPlaySearch();
            int? nullable;
            int num1;
            if (!Tables.HasValue)
            {
                num1 = 0;
            }
            else
            {
                nullable = Tables;
                num1 = nullable.Value;
            }
            int num2 = num1;
            nullable = Random;
            int num3;
            if (!nullable.HasValue)
            {
                num3 = 0;
            }
            else
            {
                nullable = Random;
                num3 = nullable.Value;
            }
            int num4 = num3;
            int num5 = RouletteGameType == GameType.Random ? 1 : (SelectedAutoplay == null ? 1 : SelectedAutoplayValue);
            _tables.SetTotalCalculatedSpins(num2 * num4 * num5);
            List<Task> tasks = new();
            nullable = Tables;
            if (nullable.HasValue)
            {
                int tableId = 1;
                while (true)
                {
                    int num6 = tableId;
                    nullable = Tables;
                    int num7 = nullable.Value;
                    if (num6 <= num7)
                    {
                        tasks.Add(TableSpinTaskCaller(tableId, token));
                        if (!token.IsCancellationRequested)
                            ++tableId;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            await Task.WhenAll(tasks);
            lock (tasks)
            {
                int num8 = tasks.Count(t => t.IsCanceled);
                int num9 = tasks.Count(t => t.IsCompleted);
                if (num8 <= 0 && num9 != tasks.Count())
                {
                    tasks = null!;
                }
                else
                {
                    FinalizeCancellationSpin();
                    tasks = null!;
                }
            }
        }

        private Task TableSpinTaskCaller(int tableId, CancellationToken token) => Task.Run(async () => await RunTableAutoplays(tableId, token));

        private void FinalizeCancellationSpin()
        {
            if (SpinProgress == 100.0)
                return;
            SpinProgress = 100.0;
        }

        private async Task<Task> RunTableAutoplays(int tableId, CancellationToken token)
        {
            bool cancelled = false;
            for (int autoplay = 1; autoplay <= SelectedAutoplayValue; ++autoplay)
            {
                ProcessTableAutoplayWrapper(tableId, autoplay, token);
                if (token.IsCancellationRequested)
                {
                    int num = await Task.Run(() => cancelled = true) ? 1 : 0;
                    break;
                }
            }
            return !cancelled ? Task.CompletedTask : Task.FromCanceled(token);
        }

        public async void ProcessTableAutoplayWrapper(
          int tableId,
          int autoplay,
          CancellationToken token)
        {
            await ProcessTableAutoplayTask(tableId, autoplay, token);
        }

        public async Task<Task> ProcessTableAutoplayTask(
          int tableId,
          int autoplay,
          CancellationToken token)
        {
            bool cancelled = false;
            lock (_tables)
            {
                Table table = _tables.Get(tableId, autoplay)!;
                if (table == null)
                {
                    table = new Table(_engineService)
                    {
                        TableId = tableId,
                        Autoplay = autoplay,
                        R1Wlimit = R1WLimit,
                        TWlimit = TWLimit
                    };
                    _tables.AddTable(table);
                    _engineService.AddGamePhaseAsync(table.UniqueTableId, table.Game.GameId, 0, (int)RouletteGameType);
                }

                lock (table)
                {
                    if (!table.ExactMatch)
                    {
                        if (!table.DoneSpinning)
                        {
                            if (Random.HasValue)
                            {                                
                                //_engineService.SetGameBoardSpinType(table.UniqueTableId, table.Game.RouletteGameId, (int)RouletteGameType);
                                //_engineService.SetGameBoardCountStyle(table.UniqueTableId, table.Game.RouletteGameId);
                                //_engineService.AddBoardPhase(table.UniqueTableId, table.Game.RouletteGameId, (int)RouletteGameType);
                                ProcessTableAutoplaySpin(table, Random.Value, token);
                            }
                        }
                    }
                }                
            }
            int num = await Task.Run(() => cancelled = token.IsCancellationRequested) ? 1 : 0;
            return !cancelled ? Task.CompletedTask : Task.FromCanceled(token);
        }

        public void ProcessTableAutoplaySpin(Table spinTable, int totalSpins, CancellationToken token)
        {
            lock (_tables)
            {
                lock (spinTable)
                {
                    lock (spinTable.Game)
                    {
                        //Debug.WriteLine($"SPINNING FOR:\tTable: {spinTable.UniqueTableId}\tGame: {spinTable.Game.RouletteGameId}");

                        for (int index = 1; index <= totalSpins; ++index)
                        {
                            if (spinTable.DoneSpinning)
                            {
                                _tables.AddDoneSpins(totalSpins - spinTable.Spins);
                                SpinProgress = _tables.GetCurrentPercentage();
                                break;
                            }

                            spinTable.Game.AddSpinTypeHistory((int)RouletteGameType);
                            var currSpinNo = spinTable.Game.CurrentSpinNo + 1;
                            spinTable.Game.UpdateCurrentSpin(currSpinNo);
                            spinTable.Game.CaptureSpin(_numberGenerator.NextRandomNumber(), 0);
                            
                            //int resetCount = _engineService.GetGameBoardResetCount(spinTable.UniqueTableId, spinTable.Game.RouletteGameId);
                            //Debug.WriteLine($"Spin: {index}/{totalSpins}\tBoard Resets: {resetCount}");

                            _tables.AddOverallSpin();
                            SpinProgress = _tables.GetCurrentPercentage();
                            spinTable.ExactMatch = CheckLimit(spinTable);
                            spinTable.WinsMatch = CheckWinsLimit(spinTable);
                            if (index == totalSpins || spinTable.ExactMatch)
                                spinTable.DoneSpinning = true;
                            if (index == totalSpins || spinTable.ExactMatch || spinTable.WinsMatch)
                                _searches.AddSpinResult(spinTable);
                            if (token.IsCancellationRequested)
                                break;
                            ExactMatchCount = _searches.GetSpinResultsExactMatchCount();
                            R1WMatchCount = _searches.GetSpinResultsR1WMatchCount();
                            TWMatchCount = _searches.GetSpinResultsTWMatchCount();
                        }
                        //Debug.WriteLine("");
                        //Debug.WriteLine("");
                    }
                }
            }
        }

        public async Task WaitTillDoneSpinning()
        {
            //await _engineService.WritePhasesAsync();
        }

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
                }).Count(wl => wl) > 0;
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
    }
}

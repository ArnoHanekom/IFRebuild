using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Engine;
using Infinity.Engine.Services;
using Infinity.Roulette.LayoutModels;
using Infinity.Roulette.Statics;
using Infinity.Services.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infinity.Roulette.ViewModels
{
    public class NewTableViewModel : ViewModelBase
    {
        private readonly ITableService _tables;
        private readonly INumberGenerator _numberGenerator;
        private readonly ITableSettingService _settingService;
        private readonly IGameTypeService _gameTypeService;
        private readonly IConcurrentSearchService _searches;
        private readonly ICountColorService _countColorService;
        private readonly IEngineService _engineService;
        //private readonly IOddWinService _oddWinService;
        private CancellationTokenSource cancellationToken;
        private Func<int, int, bool> WinsLimitReached = (int val, int valLimit) => val >= valLimit;
        private Func<int, int, bool> LimitReached = (int val, int valLimit) => val == valLimit;
        public List<IObserver<Table>> observers = new();

        public NewTableViewModel(ITableService tables, INumberGenerator numberGenerator, ITableSettingService settingService, IGameTypeService gameTypeService,
          IConcurrentSearchService searches, ICountColorService countColorService, IEngineService engineService)
        {
            _tables = tables;
            _numberGenerator = numberGenerator;
            _settingService = settingService;
            _gameTypeService = gameTypeService;
            _searches = searches;
            _countColorService = countColorService;
            _engineService = engineService;
            
            cancellationToken = new CancellationTokenSource();            
        }

        public void SetOtherCountStyle()
        {
            Style otherStyle = (Style)Application.Current.FindResource("BetWindowLabelTextBlockOtherSpinCount");
            _countColorService.SetOtherStyle(otherStyle);
        }

        private TableSetting _tableSetting { get; set; } = default!;

        public TableSetting TableSetting
        {
            get => _tableSetting;
            set
            {
                if (_tableSetting != value)
                    _tableSetting = value;
                OnPropertyChanged(nameof(TableSetting));
            }
        }

        private TableGameSetting _tableGameSetting { get; set; } = default!;

        public TableGameSetting TableGameSetting
        {
            get => _tableGameSetting;
            set
            {
                if (_tableGameSetting != value)
                    _tableGameSetting = value;
                OnPropertyChanged(nameof(TableGameSetting));
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

        private int _gameRows { get; set; }

        public int GameRows
        {
            get => _gameRows;
            set
            {
                if (_gameRows != value)
                    _gameRows = value;
                OnPropertyChanged(nameof(GameRows));
            }
        }

        private int _gameCounts { get; set; }

        public int GameCounts
        {
            get => _gameCounts;
            set
            {
                if (_gameCounts != value)
                    _gameCounts = value;
                OnPropertyChanged(nameof(GameCounts));
            }
        }

        private int _gameGS { get; set; }

        public int GameGS
        {
            get => _gameGS;
            set
            {
                if (_gameGS != value)
                    _gameGS = value;
                OnPropertyChanged(nameof(GameGS));
            }
        }

        private int _gameMaxGS { get; set; }

        public int GameMaxGS
        {
            get => _gameMaxGS;
            set
            {
                if (_gameMaxGS != value)
                    _gameMaxGS = value;
                OnPropertyChanged(nameof(GameMaxGS));
            }
        }

        private Table _GameTable { get; set; } = default!;

        public Table GameTable
        {
            get => _GameTable;
            set
            {
                if (_GameTable != value)
                    _GameTable = value;
                OnPropertyChanged(nameof(GameTable));
            }
        }

        private GameType _ChosenGameType { get; set; } = default!;

        public GameType ChosenGameType
        {
            get => _ChosenGameType;
            set
            {
                if (_ChosenGameType != value)
                    _ChosenGameType = value;
                LoadGameTypeSettings();
                OnPropertyChanged(nameof(ChosenGameType));
            }
        }

        private ComboBoxItem _selectedGameType { get; set; } = default!;

        public ComboBoxItem SelectedGameType
        {
            get => _selectedGameType;
            set
            {
                if (_selectedGameType != value)
                    _selectedGameType = value;
                if (_selectedGameType != null)
                {
                    IsAutoplay = _selectedGameType.Content.ToString() == GameType.Autoplay.ToString();
                    ChosenGameType = _gameTypeService.Get(_selectedGameType.Content.ToString()!);
                }
                OnPropertyChanged(nameof(SelectedGameType));
            }
        }

        private ObservableCollection<ComboBoxItem> _gameTypeOptions { get; set; } = default!;

        public ObservableCollection<ComboBoxItem> GameTypeOptions
        {
            get => _gameTypeOptions;
            set
            {
                if (_gameTypeOptions != value)
                    _gameTypeOptions = value;
                OnPropertyChanged(nameof(GameTypeOptions));
            }
        }

        private int _totalGameSpins { get; set; }

        public int TotalGameSpins
        {
            get => _totalGameSpins;
            set
            {
                if (_totalGameSpins != value)
                    _totalGameSpins = value;
                OnPropertyChanged(nameof(TotalGameSpins));
            }
        }

        private RouletteGame _tableGame { get; set; } = default!;

        public RouletteGame TableGame
        {
            get => _tableGame;
            set
            {
                if (_tableGame != value)
                    _tableGame = value;
                if (_tableGame != null)
                {
                    List<int> spinHistory = new(_tableGame.SpinHistory.TakeLast(100));
                    spinHistory.Reverse();
                    GameHistory = new List<Label>(new GameSpinHistory(spinHistory));
                    TotalGameSpins = _tableGame.Spins;
                    GameRows = _tableGame.GetRows();
                    GameCounts = _tableGame.GetCounts();
                    GameGS = _tableGame.GetGS();
                    GameMaxGS = _tableGame.GetMaxGS();                    
                }
                OnPropertyChanged(nameof(TableGame));
            }
        }

        private bool _StartNewTable { get; set; }

        public bool StartNewTable
        {
            get => _StartNewTable;
            set
            {
                if (_StartNewTable != value)
                    _StartNewTable = value;
                OnPropertyChanged(nameof(StartNewTable));
            }
        }

        public void LoadDefaults()
        {
            IsReset = false;
            Spinning = false;
            GameTypeOptions = new ObservableCollection<ComboBoxItem>(_gameTypeService.GetTypes().Where(gt => gt != GameType.Spinfile && gt != 0).Select(gt =>
            {
                return new ComboBoxItem(){Content = gt.ToString()};
            }));
            SelectedGameType = _gameTypeOptions.FirstOrDefault(gto => gto.Content.ToString() == GameType.Random.ToString())!;            
            PrepareAutoplayOptions();
            CheckAndLoadAutoplaySettings();
            GetLatestSetting();
            if (_setting == null || _setting.GameSettings.Count() <= 0)
                return;
            if (_setting.DefaultGameType != GameType.None)
                SelectedGameType = _gameTypeOptions.FirstOrDefault(gto => gto.Content.ToString() == _setting.DefaultGameType.ToString())!;
            LoadGameTypeSettings();
        }

        private void LoadResetDefaults()
        {
            Spinning = false;
            TotalGameSpins = 0;
            GameTable = new(_engineService);
        }

        public async Task<Data.Models.TableGameSetting> LoadResetSettings() => await Task.Run(() =>
        {
            GetLatestSetting();
            if (Setting != null && Setting.GameSettings.Count() > 0)
                LoadGameSetting(ChosenGameType);
            return GameSetting;
        });

        private List<Label> _gameHistory { get; set; } = default!;

        public List<Label> GameHistory
        {
            get => _gameHistory;
            set
            {
                if (_gameHistory != value)
                    _gameHistory = value;
                OnPropertyChanged(nameof(GameHistory));
            }
        }

        private bool _IsAutoplay { get; set; }

        public bool IsAutoplay
        {
            get => _IsAutoplay;
            set
            {
                if (_IsAutoplay != value)
                    _IsAutoplay = value;
                OnPropertyChanged(nameof(IsAutoplay));
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
                int result;
                if (_SelectedAutoplay != null && int.TryParse(_SelectedAutoplay.Content.ToString(), out result))
                    SelectedAutoplayValue = result;
                OnPropertyChanged(nameof(SelectedAutoplay));
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

        private List<int> _AutoplayPrepList { get; set; } = default!;

        public List<int> AutoplayPrepList
        {
            get => _AutoplayPrepList;
            set
            {
                if (_AutoplayPrepList != value)
                    _AutoplayPrepList = value;
                OnPropertyChanged(nameof(AutoplayPrepList));
            }
        }

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

        private void PopulateAutoplayList() => AutoplayOptions = AutoplayPrepList.Select(c =>
        {
            if (c == 10)
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

        private bool _DisplayRandom { get; set; }

        public bool DisplayRandom
        {
            get => _DisplayRandom;
            set
            {
                if (_DisplayRandom != value)
                    _DisplayRandom = value;
                OnPropertyChanged(nameof(DisplayRandom));
            }
        }

        private bool _DisplayManual { get; set; }

        public bool DisplayManual
        {
            get => _DisplayManual;
            set
            {
                if (_DisplayManual != value)
                    _DisplayManual = value;
                OnPropertyChanged(nameof(DisplayManual));
            }
        }

        private int? _Manual { get; set; }

        public int? Manual
        {
            get => _Manual;
            set
            {
                int? manual = _Manual;
                int? nullable = value;
                if (!(manual.GetValueOrDefault() == nullable.GetValueOrDefault() & manual.HasValue == nullable.HasValue))
                    _Manual = value;
                OnPropertyChanged(nameof(Manual));
            }
        }

        private int? _Random { get; set; }

        public int? Random
        {
            get => _Random;
            set
            {
                int? random = _Random;
                int? nullable = value;
                if (!(random.GetValueOrDefault() == nullable.GetValueOrDefault() & random.HasValue == nullable.HasValue))
                    _Random = value;
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

        private bool _spinning { get; set; }

        public bool Spinning
        {
            get => _spinning;
            set
            {
                if (_spinning != value)
                    _spinning = value;                
                OnPropertyChanged(nameof(Spinning));
                OnPropertyChanged(nameof(NotSpinning));
            }
        }

        public bool NotSpinning => !Spinning;

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

        private bool _IsManual { get; set; }

        public bool IsManual
        {
            get => _IsManual;
            set
            {
                if (_IsManual != value)
                    _IsManual = value;
                OnPropertyChanged(nameof(IsManual));
            }
        }

        public void StartSpin()
        {

            bool canSpin = ((!IsManual) ? Random.HasValue : Manual.HasValue);
            if (!canSpin || GameTable == null || GameTable?.Game == null)
            {
                return;
            }
            bool canPlay = true;
            if (!IsManual)
            {
                GameTable.ExactMatch = CheckLimit(GameTable);
                canPlay = !GameTable.ExactMatch;
            }
            if (!canPlay)
            {
                return;
            }
            cancellationToken = new CancellationTokenSource();
            SpinProgress = 0.0;
            Spinning = true;
            if (IsManual)
            {
                StartManualSpin();
                return;
            }
            switch (ChosenGameType)
            {
                case GameType.Random:
                    StartRandomSpin(cancellationToken.Token);
                    break;
                case GameType.Autoplay:
                    StartAutoplaySpin(cancellationToken.Token);
                    break;
            }

            
        }

        private async void StartManualSpin() => await Task.Run(() => RunManualSpin());

        private void RunManualSpin()
        {
            if (Manual.HasValue)
            {
                //_engineService.SetGameBoardSpinType(GameTable.UniqueTableId, GameTable.Game.RouletteGameId, (int)GameType.Manual);
                GameTable.Game.AddSpinTypeHistory((int)GameType.Manual);
                var currSpinNo = GameTable.Game.CurrentSpinNo + 1;
                GameTable.Game.UpdateCurrentSpin(currSpinNo);
                GameTable.Game.CaptureSpin(Manual.Value);
                GameTable.ExactMatch = CheckLimit(GameTable);
                observers.ForEach(observer => observer.OnNext(GameTable));
            }
            Spinning = false;
            observers.ForEach(observer => observer.OnCompleted());
        }

        private async void StartRandomSpin(CancellationToken token) => await RunRandomSpins(token);

        private async Task RunRandomSpins(CancellationToken token)
        {
            if (Random.HasValue)
            {
                for (int i = 0; i < Random.Value; i++)
                {
                    await ProcessRandomSpins(i + 1, token);
                    if (token.IsCancellationRequested)
                        break;
                }
                UpdateRandomProgress(Random.Value);
                Spinning = false;
                observers.ForEach((observer) => observer.OnCompleted());
            }
        }

        private async Task ProcessRandomSpins(int spinno, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            await Task.Run(() =>
            {
                lock (GameTable)
                {
                    lock (GameTable.Game)
                    {
                        lock (observers)
                        {
                            GameTable.Game.AddSpinTypeHistory((int)GameType.Random);
                            var currSpinNo = GameTable.Game.CurrentSpinNo + 1;
                            GameTable.Game.UpdateCurrentSpin(currSpinNo);
                            GameTable.Game.CaptureSpin(RandomNumberGenerator.NextRandomNumber());
                            GameTable.ExactMatch = CheckLimit(GameTable);
                            GameTable.WinsMatch = CheckWinsLimit(GameTable);
                            UpdateRandomProgress(spinno);
                            observers.ForEach(observer => observer.OnNext(GameTable));
                            if (!GameTable.ExactMatch && !GameTable.WinsMatch)
                                return;
                            cancellationToken.Cancel();
                        }
                    }
                }
            });
        }

        private void UpdateRandomProgress(int spinno)
        {
            if (!Random.HasValue)
                return;
            SpinProgress = Math.Round(spinno / (double)Random.Value * 100.0, 2);
        }

        private async void StartAutoplaySpin(CancellationToken token) => await RunAutoplaySpins(token);

        private async Task RunAutoplaySpins(CancellationToken token)
        {
            if (SelectedAutoplayValue <= 1)
                return;
            for (int i = 0; i < SelectedAutoplayValue; ++i)
            {
                await RunAutoplayRandomSpins(i + 1, token);
                if (token.IsCancellationRequested)
                    break;
            }
            Spinning = false;
            observers.ForEach(observer => observer.OnCompleted());
        }

        private async Task RunAutoplayRandomSpins(int autoplayno, CancellationToken token)
        {
            if (Random.HasValue)
            {
                for (int i = 0; i < Random.Value; i++)
                {
                    await ProcessAutoplayRandomSpin(autoplayno, i + 1, Random.Value, (Random.Value * SelectedAutoplayValue), token);
                    if (token.IsCancellationRequested)
                        break;
                }
            }            
        }

        private async Task ProcessAutoplayRandomSpin(int autoplayno, int spinno, int spins, int total, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            await Task.Run(() =>
            {
                lock (GameTable)
                {
                    lock (GameTable.Game)
                    {
                        lock (observers)
                        {
                            _GameTable.Game.AddSpinTypeHistory((int)GameType.Autoplay);
                            var currSpinNo = _GameTable.Game.CurrentSpinNo + 1;
                            _GameTable.Game.UpdateCurrentSpin(currSpinNo);
                            _GameTable.Game.CaptureSpin(RandomNumberGenerator.NextRandomNumber());
                            _GameTable.ExactMatch = CheckLimit(_GameTable);
                            _GameTable.WinsMatch = CheckWinsLimit(_GameTable);
                            UpdateAutoplayProgress((autoplayno - 1) * spins + spinno, total);
                            observers.ForEach(observer => observer.OnNext(_GameTable));
                            if (!_GameTable.ExactMatch && !_GameTable.WinsMatch)
                                return;
                            cancellationToken.Cancel();
                        }
                    }
                }
            });
        }

        private void UpdateAutoplayProgress(int spinno, int total)
        {
            if (!Random.HasValue)
                return;
            SpinProgress = Math.Round(spinno / (double)total * 100.0, 2);
        }

        public async void StopSpins() => await Task.Run(() => StopTableRuns());

        private void StopTableRuns()
        {
            if (cancellationToken != null)
                cancellationToken.Cancel();
            SpinProgress = 100.0;
            observers.ForEach(observer => observer.OnCompleted());
        }

        private bool _isReset { get; set; }

        public bool IsReset
        {
            get => _isReset;
            set
            {
                if (_isReset != value)
                    _isReset = value;
                OnPropertyChanged(nameof(IsReset));
            }
        }

        public void ResetTable()
        {
            _countColorService.ResetOtherStyle();
            _isReset = true;
            StartNewTable = true;
            GameSetting = default!;
            RowLimit = new int?();
            CountLimit = new int?();
            GSLimit = new int?();
            Random = new int?();
            Manual = new int?();
            R1WLimit = new int?();
            TWLimit = new int?();
            LoadResetDefaults();
            observers.ForEach(observer => observer.OnCompleted());
        }

        public async Task<CancellationToken> GetNewCancellationToken()
        {
            cancellationToken = new CancellationTokenSource();
            return await Task.Run(() => cancellationToken.Token);
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
                Table table1 = table;
                r1Wlimit = R1WLimit;
                int? limit = new int?(r1Wlimit!.Value);
                return table1.TableR1Match(limit);
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

        public IDisposable Subscribe(IObserver<Table> observer)
        {
            Unsubscriber<Table> unsubscriber = new Unsubscriber<Table>(observers, observer);
            if (observers.Contains(observer))
                return unsubscriber;
            observers.Add(observer);
            return unsubscriber;
        }

        public async Task<List<int>> LoadSpinfileAsync(string filename) => await Task.Run(() =>
        {
            List<int> intList = new List<int>();
            using (FileStream fileStream = File.Open(filename, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string str = streamReader.ReadLine()!;
                        if (!string.IsNullOrEmpty(str))
                        {
                            foreach (string s in str.Split('\t'))
                            {
                                int result;
                                if (int.TryParse(s, out result) && result > 0)
                                    intList.Add(result);
                            }
                        }
                    }
                }
            }
            return intList;
        });

        public void StartSpinfile()
        {
            if (GameTable?.Game == null)
                return;
            SpinProgress = 0.0;
            Spinning = true;
            cancellationToken = new CancellationTokenSource();
            StartSpinfileSpin(cancellationToken.Token);
        }

        private async void StartSpinfileSpin(CancellationToken token) => await RunSpinfileSpins(token);

        private async Task RunSpinfileSpins(CancellationToken token)
        {
            if (Spinfile.Count() <= 0)
                return;
            for (int i = 0; i < Spinfile.Count(); ++i)
            {
                await RunSpinfileSpin(i, token);
                if (token.IsCancellationRequested)
                    break;
            }
            Spinning = false;
            observers.ForEach(observer => observer.OnCompleted());
        }

        private async Task RunSpinfileSpin(int spinFileIdx, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            await ProcessRunSpinfileSpin(spinFileIdx, spinFileIdx + 1, Spinfile.Count(), token);
        }

        private async Task ProcessRunSpinfileSpin(
          int spinFileIdx,
          int spinno,
          int total,
          CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            await Task.Run(() =>
            {
                lock (GameTable)
                {
                    lock (GameTable.Game)
                    {
                        lock (observers)
                        {
                            lock (Spinfile)
                            {
                                //_engineService.SetGameBoardSpinType(GameTable.UniqueTableId, GameTable.Game.RouletteGameId, (int)GameType.Spinfile);
                                GameTable.Game.AddSpinTypeHistory((int)GameType.Spinfile);
                                var currSpinNo = GameTable.Game.CurrentSpinNo + 1;
                                GameTable.Game.UpdateCurrentSpin(currSpinNo);
                                GameTable.Game.CaptureSpin(Spinfile[spinFileIdx]);
                                UpdateSpinfileProgress(spinno, total);
                                observers.ForEach(observer => observer.OnNext(GameTable));
                            }
                        }
                    }
                }
            });
        }

        private void UpdateSpinfileProgress(int spinno, int total) => SpinProgress = Math.Round(spinno / (double)total * 100.0, 2);

        private TableSetting _setting { get; set; } = default!;

        public TableSetting Setting
        {
            get => _setting;
            set
            {
                if (_setting != value)
                    _setting = value;
                OnPropertyChanged(nameof(Setting));
            }
        }

        private Data.Models.TableGameSetting _gameSetting { get; set; } = default!;

        public Data.Models.TableGameSetting GameSetting
        {
            get => _gameSetting;
            set
            {
                if (_gameSetting != value)
                    _gameSetting = value;
                OnPropertyChanged(nameof(GameSetting));
            }
        }

        public void LoadGameSetting(GameType typeToLoad)
        {
            if (Setting == null)
                return;
            GameSetting = Setting.GameSettings.FirstOrDefault(gs => gs.Type == _gameTypeService.Get(typeToLoad.ToString()))!;
        }

        public void ReloadSettings(TableSetting newSetting)
        {
            Setting = newSetting;
            if (Setting == null)
                return;
            if (Setting.DefaultGameType != GameType.None)
                SelectedGameType = GameTypeOptions.FirstOrDefault(gto => gto.Content.ToString() == Setting.DefaultGameType.ToString())!;
            LoadGameTypeSettings();
        }

        public async Task<Data.Models.TableGameSetting?> GetCurrentSetting() => await Task.Run(() =>
        {
            LoadGameSetting(ChosenGameType);
            return GameSetting;
        });

        public void LoadGameTypeSettings()
        {
            LoadGameSetting(ChosenGameType);
            if (GameSetting != null)
            {
                OverwriteGameTypeDefaults();
            }
            else
            {
                Random = new int?();
                RowLimit = new int?();
                CountLimit = new int?();
                GSLimit = new int?();
                R1WLimit = new int?();
                TWLimit = new int?();
            }
        }

        public void GetLatestSetting() => Setting = _settingService.Get();

        private void OverwriteGameTypeDefaults()
        {
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
            int autoplaySetting = GameSetting?.AutoplayNumber ?? 10;
            AutoplayOptions = AutoplayPrepList.Select(delegate (int c)
            {
                if (c == autoplaySetting)
                {
                    SelectedAutoplay = new ComboBoxItem
                    {
                        Content = c.ToString(),
                        FontFamily = new FontFamily("Century Gothic"),
                        IsSelected = true
                    };
                    return SelectedAutoplay;
                }
                return new ComboBoxItem
                {
                    Content = c.ToString(),
                    FontFamily = new FontFamily("Century Gothic")
                };
            });
        }
    }
}

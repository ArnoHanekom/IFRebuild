using Infinity.Data.Constants;
using Infinity.Roulette.Rework.Data.Models;
using Infinity.Roulette.Rework.Services;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infinity.Roulette.ViewModels;

public class ReworkViewModel : ViewModelBase
{
    private readonly IPlayService _playService;
    private readonly IReworkSettingService _reworkSettingService;

    public CancellationTokenSource CancellationSource { get; set; } = new();

    private int? _tablesToPlay {  get; set; }
    private int? _randomize { get; set; }
    private List<int> _autoplayOptions
    {
        get
        {
            var options = new List<int>();
            for (int index = 10; index <= 100; index += 10)
                options.Add(index);
            for (int index = 120; index <= 200; index += 20)
                options.Add(index);
            for (int index = 250; index <= 300; index += 50)
                options.Add(index);
            return options;
        }
    }
    private int? _rowLimit { get; set; }
    private int? _r1wLimit { get; set; }
    private int? _twLimit { get; set; }
    private int? _countLimit { get; set; }
    private int? _gsLimit { get; set; }
    private ComboBoxItem? _selectedAutoplay { get; set; }
    private IEnumerable<ComboBoxItem> _autoplays
    {
        get
        {
            return AutoplayOptions.Select(ao =>
            {
                if (ao == SelectedAutoplayValue || ao == CurrentSetting.Autoplay)
                {
                    SelectedAutoplay = new ComboBoxItem()
                    {
                        Content = ao.ToString(),
                        FontFamily = new FontFamily("Century Gothic"),
                        IsSelected = true
                    };
                    return SelectedAutoplay;
                }

                return new ComboBoxItem()
                {
                    Content = ao.ToString(),
                    FontFamily = new FontFamily("Century Gothic")
                };
            });
        }
    }
    private int _selectedAutoplayValue
    {
        get
        {
            if (SelectedAutoplay is null)
                return 1;

            if (int.TryParse(SelectedAutoplay.Content.ToString(), out int selected))
                return selected;

            return 1;
        }
    }
    private GameType _rouletteGameType { get; set; } = GameType.None;
    private bool _showStopBtn { get; set; } = false;
    private bool _showRunBtn { get; set; } = true;

    public int? TablesToPlay
    {
        get { return _tablesToPlay; }
        set
        {
            _tablesToPlay = value;
            OnPropertyChanged(nameof(TablesToPlay));
        }
    }
    public int? Randomize
    {
        get { return _randomize; }
        set
        {
            _randomize = value;
            OnPropertyChanged(nameof(Randomize));
        }
    }
    public int? RowLimit
    {
        get { return _rowLimit; }
        set
        {
            _rowLimit = value;
            OnPropertyChanged(nameof(RowLimit));
        }
    }
    public int? R1WLimit
    {
        get { return _r1wLimit; }
        set
        {
            _r1wLimit = value;
            OnPropertyChanged(nameof(R1WLimit));
        }
    }
    public int? TWLimit
    {
        get { return _twLimit; }
        set
        {
            _twLimit = value;
            OnPropertyChanged(nameof(TWLimit));
        }
    }
    public int? CountLimit
    {
        get { return _countLimit; }
        set
        {
            _countLimit = value;
            OnPropertyChanged(nameof(CountLimit));
        }
    }
    public int? GSLimit
    {
        get { return _gsLimit; }
        set
        {
            _gsLimit = value;
            OnPropertyChanged(nameof(GSLimit));
        }
    }
    public ComboBoxItem? SelectedAutoplay
    {
        get { return _selectedAutoplay; }
        set
        {
            _selectedAutoplay = value;
            OnPropertyChanged(nameof(SelectedAutoplay));
        }
    }
    public List<int> AutoplayOptions
    {
        get { return _autoplayOptions; }
    }
    public int SelectedAutoplayValue
    {
        get { return _selectedAutoplayValue; }
    }
    public IEnumerable<ComboBoxItem> Autoplays
    {
        get { return _autoplays; }
    }
    public GameType RouletteGameType
    {
        get { return _rouletteGameType; }
        set
        {
            _rouletteGameType = value;
            OnPropertyChanged(nameof(RouletteGameType));
        }
    }
    public bool ShowStopBtn
    {
        get { return _showStopBtn; }
        set
        {
            _showStopBtn = value;
            OnPropertyChanged(nameof(ShowStopBtn));
        }
    }
    public bool ShowRunBtn
    {
        get { return _showRunBtn; }
        set
        {
            _showRunBtn = value;
            OnPropertyChanged(nameof(ShowRunBtn));
        }
    }
    public int PageSize { get; set; } = 100;

    private ObservableCollection<Pager> _pageControls { get; set; } = new();
    public ObservableCollection<Pager> PageControls
    {
        get
        {
            return _pageControls;
        }
        set
        {
            _pageControls = value;
            OnPropertyChanged(nameof(PageControls));
        }
    }
    public SpinType SpinType { get; set; }
    public IProgress<double> ProgressReporter { get; set; } = default!;
    public IProgress<double> PrepareReporter { get; set; } = default!;
    private SearchResult[] _searchResults { get; set; } = Array.Empty<SearchResult>();
    private ReworkTableSetting _currentSetting { get; set; } = new() { TablesToPlay = default, Autoplay = default, Mode = default, Randomize = default };
    public ReworkTableSetting CurrentSetting
    {
        get
        {
            return _currentSetting;
        }
        set
        {
            _currentSetting = value;
            OnPropertyChanged(nameof(CurrentSetting));
        }
    }
    public ReworkViewModel(IPlayService playService, IReworkSettingService reworkSettingService)
    {
        _playService = playService;
        _reworkSettingService = reworkSettingService;
    }
    public async Task SetReportersAsync(CancellationToken ct = default)
    {
        await _playService.SetPrepareReporterAsync(PrepareReporter, ct);
        await _playService.SetProgressReporterAsync(ProgressReporter, ct);
    }
    public async Task PreparePlaysAsync(CancellationToken ct = default)
    {
        if (TablesToPlay.HasValue)
        {
            if (RouletteGameType == GameType.Random)
                await _playService.PrepareAutoplaysAsync(1, TablesToPlay.Value, ct);
            else
                await _playService.PrepareAutoplaysAsync(SelectedAutoplayValue, TablesToPlay.Value, ct);
        }
    }
    public async Task PrintHistoryAsync(CancellationToken ct = default)
    {
        await _playService.WriteHistoryAsync(ct);
    }
    public async Task<PagedResult> LoadResultForPageAsync(int currPage, int pageSize, CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            var skipCalc = (currPage - 1) * pageSize;
            var pagedResult = new PagedResult()
            {
                TotalRecords = _searchResults.Length,
                CurrentPage = currPage,
                PageSize = pageSize,
                ResultItems = _searchResults.Skip(skipCalc).Take(pageSize).ToArray()
            };

            return pagedResult;
        }, ct);
    }
   
    private async Task<int> GetPageCountAsync(CancellationToken ct = default)
    {
        var totalResults = _searchResults.Length;
        var pageRemainder = totalResults % PageSize;
        return await Task.Run(() =>
        {
            return pageRemainder == 0 ? totalResults / PageSize : (totalResults / PageSize) + 1;
        }, ct);
    }
    public async Task ChangePageAsync(int selectedPage, CancellationToken ct = default)
    {
        var pages = await GetPageCountAsync(ct).ConfigureAwait(false);
        PageControls = new ObservableCollection<Pager>(await SetupPagesAsync(pages, selectedPage, ct).ConfigureAwait(false));
    }
    private async Task<List<Pager>> SetupPagesAsync(int pages, int selectedPage = 1, CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            List<Pager> _pages = new();
            for (int p = 1; p <= pages; p++)
            {
                _pages.Add(new() { PageNumber = p, IsSelected = p == selectedPage });
            }
            return _pages;
        }, ct);
    }    

    public async Task ProcessSpinfileSpinsAsync(List<int> spinNumbers, CancellationToken ct = default)
    {
        await _playService.RunAutoplaysTablesSpinfileAsync(spinNumbers, SpinType, ct).ConfigureAwait(false);
    }
    public async Task GetNewCancellationAsync(CancellationToken ct = default)
    {
        await Task.Run(() =>
        {
            CancellationSource = new();
        }, ct);
    }
    public async Task StartDashboardSearchAsync(CancellationToken ct = default)
    {
        if (TablesToPlay.HasValue)
        {
            if (Randomize.HasValue)
            {
                if (RouletteGameType == GameType.Random)
                    await _playService.PlayAsync(1, TablesToPlay.Value, Randomize.Value, SpinType.Random, ct);
                else
                    await _playService.PlayAsync(SelectedAutoplayValue, TablesToPlay.Value, Randomize.Value, SpinType.Random, ct);
            }
        }
        //while (!ct.IsCancellationRequested)
        //{
        //    await Task.Delay(100);
        //}        
    }
    public class PagedResult
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public SearchResult[] ResultItems { get; set; } = Array.Empty<SearchResult>();
    }
    public async Task ResetDashboardAsync(CancellationToken ct = default)
    {
        await _playService.ResetSearchAsync(ct);
        await ClearSettingsAsync(ct);
        await LoadDefaultsAsync(ct);
    }

    public async Task ClearSettingsAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(ClearSettings, ct);
        }
        catch
        {
            ClearSettings();
        }
    }

    public void GetGameTypeSettings()
    {
        var setting = _reworkSettingService.GetSettingByGameType(RouletteGameType);
        if (setting is not null) CurrentSetting = setting;
    }

    private void ClearSettings()
    {
        TablesToPlay = null;
        Randomize = null;
        RowLimit = null;
        R1WLimit = null;
        TWLimit = null;
        CountLimit = null;
        GSLimit = null;
        SelectedAutoplay = null;
        RouletteGameType = GameType.None;
    }
    public async Task LoadDefaultsAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(LoadDefaults, ct);
        }
        catch
        {
            LoadDefaults();
        }
    }

    private void LoadDefaults()
    {
        GetGameTypeSettings();
    }

    private void ReloadAutoplays()
    {

    }
}
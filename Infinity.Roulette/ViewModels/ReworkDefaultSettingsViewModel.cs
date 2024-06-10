using Infinity.Data.Constants;
using Infinity.Roulette.Rework.Data.Models;
using Infinity.Roulette.Rework.Services;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infinity.Roulette.ViewModels;

public class ReworkDefaultSettingsViewModel : ViewModelBase
{
    private readonly IReworkSettingService _reworkSettingService;

    private IEnumerable<ComboBoxItem> _gameTypes { get; set; } = default!;
    public IEnumerable<ComboBoxItem> GameTypes
    {
        get { return _gameTypes; }
        set
        {
            _gameTypes = value;
            OnPropertyChanged(nameof(GameTypes));
        }
    }
    private ComboBoxItem? _selectedGameType { get; set; } = default!;
    public ComboBoxItem? SelectedGameType
    {
        get { return _selectedGameType; }
        set
        {
            _selectedGameType = value;
            OnPropertyChanged(nameof(SelectedGameType));
        }
    }
    private int? _tablesToPlay { get; set; }
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
    private int _savedAutoplay { get; set; } = 1;
    public int SavedAutoplay
    {
        get { return _savedAutoplay; }
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
                if (ao == SelectedAutoplayValue || ao == SavedAutoplay)
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
    public bool IsAutoplaySettings
    {
        get
        {
            return SelectedGameType is null || SelectedGameType.Content.ToString()!.ToLower() == "random" ? false : true;
        }
    }
    public Visibility IsAutoplayVisible
    {
        get { return IsAutoplaySettings ? Visibility.Visible : Visibility.Collapsed; }
    }
    private ReworkTableSetting[] _defaultSettings { get; set; } = new ReworkTableSetting[2];
    public ReworkTableSetting[] DefaultSettings
    {
        get
        {
            return _defaultSettings;
        }
        set
        {
            _defaultSettings = value;
            OnPropertyChanged(nameof(DefaultSettings));
        }
    }    
    private ReworkTableSetting _currentSetting { get ; set; }
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
    public ReworkDefaultSettingsViewModel(IReworkSettingService reworkSettingService)
    {
        _reworkSettingService = reworkSettingService;
        _defaultSettings[0] = new ReworkTableSetting() { TablesToPlay = default, Autoplay = default, Randomize = default, Mode = GameType.Random };
        _defaultSettings[1] = new ReworkTableSetting() { TablesToPlay = default, Autoplay = default, Randomize = default, Mode = GameType.Autoplay };        
        LoadDefaultPage();
        _currentSetting = _defaultSettings[0];
    }
    private void LoadDefaultPage()
    {
        PopulateGameTypes();
        PopulateSelectedGameTypeSettings();
        LoadSavedSettings();
    }
    private void PopulateGameTypes()
    {
        var gameTypes = new List<ComboBoxItem>();
        foreach (var gt in Enum.GetValues(typeof(GameType)))
        {
            if (gt.ToString()!.ToLower() == "random" || gt.ToString()!.ToLower() == "autoplay")
            {
                if (gt.ToString() == "Random")
                {
                    SelectedGameType = new ComboBoxItem()
                    {
                        Content = gt.ToString(),
                        IsSelected = true,
                        FontFamily = new FontFamily("Century Gothic")
                    };
                    gameTypes.Add(SelectedGameType);
                }
                else
                    gameTypes.Add(new ComboBoxItem() { Content = gt.ToString(), FontFamily = new FontFamily("Century Gothic") });
            }
        }
        GameTypes = gameTypes.Select(gt => gt);
    }
    private void PopulateSelectedGameTypeSettings()
    {
        if (Enum.TryParse(SelectedGameType?.Content.ToString() ?? "", out GameType selected))
        {
            RouletteGameType = selected;
        }        
    }
    private void LoadSavedSettings()
    {
        var savedSetting = _reworkSettingService.GetDashboardSetting();

        if (savedSetting.TableSettings.Count > 0)
        {
            try
            {
                _defaultSettings[0] = savedSetting.TableSettings[0];
            }
            catch { }
            try
            {
                _defaultSettings[1] = savedSetting.TableSettings[1];
            }
            catch { }

            //TODO: Change to use saved default game type
            var setting = savedSetting.TableSettings.FirstOrDefault(ts => ts.Mode == GameType.Random);
            if (setting is null) return;
            PresetSettings(setting);
        }
    }
    private void PresetSettings(ReworkTableSetting tableSetting)
    {
        TablesToPlay = tableSetting.TablesToPlay;
        _savedAutoplay = tableSetting.Autoplay;
        _savedAutoplay = DefaultSettings[1].Autoplay;
        Randomize = tableSetting.Randomize;
        TWLimit = tableSetting.RowLimit;
        R1WLimit = tableSetting.R1WLimit;
        RowLimit = tableSetting.RowLimit;
        CountLimit = tableSetting.CountLimit;
        GSLimit = tableSetting.GSLimit;

    }
    private void ClearAllSettings()
    {
        TablesToPlay = null;
        _savedAutoplay = 1;
        Randomize = null;
        TWLimit = null;
        R1WLimit = null;
        RowLimit = null;
        CountLimit = null;
        GSLimit = null;
    }
    public async Task DiscardAsync(CancellationToken ct = default)
    {
        await CreateDashboardSettingAsync(ct);
    }
    public async Task SaveSettingAsync(CancellationToken ct = default)
    {
        await CreateDashboardSettingAsync(ct);
        await CreateTableSettingAsync(ct);
        await WriteToFileAsync(ct);
    }
    public async Task CreateDashboardSettingAsync(CancellationToken ct = default)
    {
        await _reworkSettingService.CreateNewDashboardSettingAsync(ct);
    }
    public async Task CreateTableSettingAsync(CancellationToken ct = default)
    {
        DefaultSettings[1].Autoplay = SelectedAutoplayValue > 1 ? SelectedAutoplayValue : 0;
        foreach (var setting in DefaultSettings)
        {
            await _reworkSettingService.AddTableSettingAsync(setting, ct);
        }
    }
    public async Task WriteToFileAsync(CancellationToken ct = default)
    {
        var dashSetting = await _reworkSettingService.GetDashboardSettingAsync(ct);
        FileInfo fileInfo = new("dashboard-settings.json");
        if (fileInfo.Exists)
            fileInfo.Delete();

        using StreamWriter streamWriter = new(fileInfo.Open(FileMode.OpenOrCreate));
        streamWriter.Write(JsonConvert.SerializeObject(dashSetting));
    }
    public void LoadSelectionSettings(string selection)
    {
        if (Enum.TryParse(typeof(GameType), selection, out var selectedType))
        {
            ClearAllSettings();
            var savedSetting = _reworkSettingService.GetDashboardSetting();
            var setting = savedSetting.TableSettings.FirstOrDefault(ts => ts.Mode == (GameType)selectedType);
            if (setting is null) return;

            
            PresetSettings(setting);
        }
    }    
    public async Task ChangeCurrentSettingAsync(int changeTo)
    {
        await Task.Run(() => 
        {
            if (changeTo == 1)
            {
                _defaultSettings[0] = CurrentSetting;
                CurrentSetting = _defaultSettings[1];
            }
            else
            {
                CurrentSetting.Autoplay = SelectedAutoplayValue > 1 ? SelectedAutoplayValue : 0;
                _defaultSettings[1] = CurrentSetting;
                CurrentSetting = _defaultSettings[0];
            }
        });
    }
}
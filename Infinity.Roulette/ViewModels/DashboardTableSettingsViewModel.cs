using Infinity.Roulette.Rework.Data.Models;
using Infinity.Roulette.Rework.Services;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infinity.Roulette.ViewModels;

public class DashboardTableSettingsViewModel : ViewModelBase
{
    private readonly IReworkSettingService _reworkSettingService;
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
    private int? _tablesToPlay { get; set; }
    private int? _randomize { get; set; }
    private int? _rowLimit {  get; set; }
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
                if (ao == SelectedAutoplayValue)
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
    private bool _saved { get; set; } = false;

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
    public bool Saved
    {
        get { return _saved; }
        set
        {
            _saved = value;
            OnPropertyChanged(nameof(Saved));
        }
    }


    public DashboardTableSettingsViewModel(IReworkSettingService reworkSettingService)
    {
        _reworkSettingService = reworkSettingService;
    }

    
}
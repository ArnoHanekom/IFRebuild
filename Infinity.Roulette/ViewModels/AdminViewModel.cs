using Infinity.Services.Interfaces;
using System.Windows.Controls;

namespace Infinity.Roulette.ViewModels;

public class AdminViewModel : ViewModelBase
{
    private ComboBoxItem _selectedLicensePeriod { get; set; } = null!;                                                                 
    public ComboBoxItem SelectedLicensePeriod
    {
        get => _selectedLicensePeriod;
        set
        {
            _selectedLicensePeriod = value;
            OnPropertyChanged(nameof(SelectedLicensePeriod));
        }
    }

    private List<ComboBoxItem> _licensePeriods { get; set; } = null!;
    public List<ComboBoxItem> LicensePeriods
    {
        get => _licensePeriods;
        set
        {
            _licensePeriods = value;
            OnPropertyChanged(nameof(LicensePeriods));
        }
    }

    private readonly ILicenseService _licenseService;
    public AdminViewModel(ILicenseService licenseService)
    {
        _licenseService = licenseService;
        LoadDefaults();
    }

    public void LoadDefaults()
    {
        ComboBoxItem cb = new()
        {
            Content = "10",
            IsSelected = true
        };
        SelectedLicensePeriod = cb;

        ComboBoxItem cb2 = new()
        {
            Content = "15",
            IsSelected = false
        };
        ComboBoxItem cb3 = new()
        {
            Content = "20",
            IsSelected = false
        };
        ComboBoxItem cb4 = new()
        {
            Content = "31",
            IsSelected = false
        };

        LicensePeriods = [ cb, cb2, cb3, cb4 ];
    }

    public async Task GenerateNewLicenseAsync()
    {
        if (int.TryParse(SelectedLicensePeriod.Content.ToString(), out int selectedPeriod))
        {
            var generated = await _licenseService.GenerateLicenseAsync(selectedPeriod);
        }
    }
}
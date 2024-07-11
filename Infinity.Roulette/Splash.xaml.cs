using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Unity;

namespace Infinity.Roulette;

public partial class Splash : Window
{
    private DispatcherTimer _timer = new();
    private int _timeCounted = 0;
    private bool _checkCompleted = false;
    private bool _isValidLicense = false;

    private SplashScreenViewModel splashVM { get; set; } = default!;

    public Splash()
    {
        if (Container.container != null)
            splashVM = Container.container.Resolve<SplashScreenViewModel>();

        InitializeComponent();
        Task.Run(SetupTimerAsync);
        DoLicenseCheckAsync();
        IsValidLicenseAsync();
    }

    private async Task SetupTimerAsync()
    {
        await Dispatcher.InvokeAsync(() =>
        {
            _timer = new();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        });
    }
    private async void Timer_Tick(object? sender, EventArgs e)
    {
        await Task.Run(() =>
        {
            _timeCounted++;
        });
    }
    private async void DoLicenseCheckAsync()
    {
        while (!_checkCompleted)
        {           
            await Task.Delay(1500);
        }
        if (_checkCompleted && _isValidLicense)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                txtLicenseInfo.Text = "VALID";
                txtLicenseInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009933"));
            });
        }
        await Task.Delay(2000);
        if (_checkCompleted && _isValidLicense)
        {            
            await StartDashboardAsync();
        }

        txtLicenseInfo.Text = "NOT VALID";
        txtLicenseInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
    }
    private async Task StartDashboardAsync()
    {        
        await Dispatcher.InvokeAsync(() =>
        {
            new NewDashboard().Show();
            this.Close();
        });
    }
    private async void IsValidLicenseAsync()
    {
        await Task.Delay(2000);
        _isValidLicense = await splashVM.IsValidLicenseAsync();
        _checkCompleted = true;
    }
}
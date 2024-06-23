using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Roulette.Containers;
using Infinity.Roulette.Statics;
using Infinity.Roulette.ViewModels;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using Unity;

namespace Infinity.Roulette;
public partial class NewDashboard : Window
{
    private MainViewModel mainVM { get; set; } = default!;
    public NewDashboard()
    {
        if (Container.container != null)
            mainVM = Container.container.Resolve<MainViewModel>();
        DataContext = mainVM;
        InitializeComponent();
    }
    private async void btnPlay_Click(object sender, RoutedEventArgs e)
    {
        if (mainVM.GameSetting is not null)
        {
            await mainVM.PreparePlayStartAsync().ConfigureAwait(false);
            await mainVM.PlaySpinsAsync(mainVM.cancelToken).ConfigureAwait(false);
        }
        else
        {
            AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
            AlertNotifications.DisplayAlertMessage("Incorrect default setup. Please make sure that the default settings are setup.");
        }
    }
    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
        mainVM.Stopping = true;
        mainVM.cancelSource.Cancel();
    }
    private void mnuDefaultSettings_Click(object sender, RoutedEventArgs e)
    {
        GameSettings gameSettings = new()
        {
            Owner = this
        };
        gameSettings.ShowDialog();
    }
    public void ReloadSettings()
    {
        mainVM.ReloadSettings(SettingReload());
        mainVM.ProcessReset();
    }
    private Setting SettingReload()
    {
        FileInfo fileInfo = new("settings.json");
        if (!fileInfo.Exists)
            return mainVM.CleanDashboardSetting();
        Setting newSetting = JsonConvert.DeserializeObject<Setting>(new StreamReader(fileInfo.Open(FileMode.Open)).ReadToEnd())!;
        if (newSetting == null)
            return mainVM.CleanDashboardSetting();

        return newSetting;
    }

    private void btnResults_Click(object sender, RoutedEventArgs e)
    {
        NewSearchResults searchResults = new()
        {
            Owner = this
        };        
        searchResults.Show();
    }

    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult fullReset = AlertNotifications.ResetOptionsMessage("Would you like to reset the default settings also?");
        if (fullReset == MessageBoxResult.Yes)
        {
            mainVM.ProcessFullReset();
            AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
            AlertNotifications.DisplayAlertMessage("Dashboard and settings was reset, please set your new default settings.");
        }
        else
        {        
            mainVM.ProcessReset();
            AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
            AlertNotifications.DisplayAlertMessage("Dashboard was reset.");
        }
        ReloadSettings();
        //    ((Main)Owner).ReloadSettings();
        //    ((Main)Owner).ReloadGameSettingBindings();
    }

    private void mnuGenerateSpinfile_Click(object sender, RoutedEventArgs e)
    {

    }
}
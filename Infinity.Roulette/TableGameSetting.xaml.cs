using Infinity.Roulette.Containers;
using Infinity.Roulette.Statics;
using Infinity.Roulette.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Infinity.Roulette;

public partial class TableGameSetting : Window
{
    private TableGameSettingViewModel gameSettingsVM { get; set; } = null!;
    public TableGameSetting()
    {
        if (Container.container != null)
            gameSettingsVM = Container.container.Resolve<TableGameSettingViewModel>();
        DataContext = gameSettingsVM;
        InitializeComponent();
    }
    private void hlDefaultGameType_Click(object sender, RoutedEventArgs e)
    {
        TableGameSettingDefaultGame settingDefaultGame = new()
        {
            Owner = this
        };
        settingDefaultGame.ShowDialog();
    }
    private void cbSettingGameType_SelectionChanged(object sender, SelectionChangedEventArgs e) => gameSettingsVM.LoadSelectedSetting();
    public void UpdateDefaultGameType()
    {
        gameSettingsVM.GetLatestSetting();
        ReloadDefaultGameTypeBindings();
    }
    private static void ReloadDefaultGameTypeBindings() { }
    private static void ReloadGameSettingBindings() { }
    private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
    {
        gameSettingsVM.SaveSetting();
        ReloadGameSettingBindings();
        AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
        AlertNotifications.DisplayAlertMessage("New default settings saved.\r\nNew defaults will reload automatically.");
        ((NewTable)Owner).ReloadSettings();
        Close();
    }
    private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
}
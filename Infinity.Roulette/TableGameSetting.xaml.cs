// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.TableGameSetting
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Roulette.Containers;
using Infinity.Roulette.Statics;
using Infinity.Roulette.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Unity;


#nullable enable
namespace Infinity.Roulette
{
  public partial class TableGameSetting : Window
  {
    private  TableGameSettingViewModel gameSettingsVM { get; set; }

    public TableGameSetting()
    {
      if (Container.container != null)
        gameSettingsVM = Container.container.Resolve<TableGameSettingViewModel>();
      DataContext = gameSettingsVM;
      InitializeComponent();
    }

    private void hlDefaultGameType_Click(object sender, RoutedEventArgs e)
    {
      TableGameSettingDefaultGame settingDefaultGame = new TableGameSettingDefaultGame();
      settingDefaultGame.Owner = this;
      settingDefaultGame.ShowDialog();
    }

    private void cbSettingGameType_SelectionChanged(object sender, SelectionChangedEventArgs e) => gameSettingsVM.LoadSelectedSetting();

    public void UpdateDefaultGameType()
    {
      gameSettingsVM.GetLatestSetting();
      ReloadDefaultGameTypeBindings();
    }

    private void ReloadDefaultGameTypeBindings()
    {
    }

    private void ReloadGameSettingBindings()
    {
    }

    private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
    {
      gameSettingsVM.SaveSetting();
      ReloadGameSettingBindings();
      AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
      AlertNotifications.DisplayAlertMessage("New default settings saved.\r\nNew defaults will reload automatically.");
      ((NewTable) Owner).ReloadSettings();
      Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

  }
}

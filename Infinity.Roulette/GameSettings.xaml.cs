// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.GameSettings
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
using System.Windows.Data;
using System.Windows.Markup;
using Unity;


#nullable enable
namespace Infinity.Roulette
{
    public partial class GameSettings : Window
    {
        private GameSettingsViewModel gameSettingsVM = Container.container.Resolve<GameSettingsViewModel>();

        public GameSettings()
        {
            DataContext = gameSettingsVM;
            InitializeComponent();
        }

        private void hlDefaultGameType_Click(object sender, RoutedEventArgs e)
        {
            GameSettingDefaultGame settingDefaultGame = new GameSettingDefaultGame();
            settingDefaultGame.Owner = this;
            settingDefaultGame.ShowDialog();
        }

        private void cbSettingGameType_SelectionChanged(object sender, SelectionChangedEventArgs e) => gameSettingsVM.LoadSelectedSetting();

        public void UpdateDefaultGameType()
        {
            gameSettingsVM.GetLatestSetting();
        }

        private void ReloadGameSettingBindings() => BindingOperations.GetBindingExpression((DependencyObject)txtTables, TextBox.TextProperty)?.UpdateTarget();

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            gameSettingsVM.SaveSetting();
            ReloadGameSettingBindings();
            AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
            AlertNotifications.DisplayAlertMessage("New default settings saved.\r\nNew defaults will reload automatically.");
            if (Owner.GetType() == typeof(Main))
                ((Main)Owner).ReloadSettings();
            else
                ((NewDashboard)Owner).ReloadSettings();
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

    }
}

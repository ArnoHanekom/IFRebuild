// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Main
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Roulette.Containers;
using Infinity.Roulette.Statics;
using Infinity.Roulette.ViewModels;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Unity;

#nullable enable
namespace Infinity.Roulette
{
    public partial class Main : Window
    {
        private MainViewModel mvm { get; set; } = default!;

        public Main()
        {
            if (Container.container != null)
                mvm = Container.container.Resolve<MainViewModel>();
            DataContext = mvm;
            InitializeComponent();
            txtTables.Focus();
        }

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mvm.SetOtherCountStyle();
            if (mvm.SelectedGameType != GameType.None)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    DependencyProperty prop = TextBox.TextProperty;
                    BindingExpression binding = BindingOperations.GetBindingExpression(txtTables, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                    binding = BindingOperations.GetBindingExpression(txtRandom, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                    binding = BindingOperations.GetBindingExpression(txtRowLimit, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                    binding = BindingOperations.GetBindingExpression(txtCountLimit, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                    binding = BindingOperations.GetBindingExpression(txtGSLimit, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                    binding = BindingOperations.GetBindingExpression(txtR1W, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                    binding = BindingOperations.GetBindingExpression(txtTW, prop);
                    if (binding != null)
                        binding.UpdateTarget();
                });
                await mvm.PreparePlayStartAsync().ConfigureAwait(false);
                await mvm.PlaySpinsAsync(mvm.cancelToken).ConfigureAwait(false);
            }
            else
            {
                AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
                AlertNotifications.DisplayAlertMessage("Please select a spin type");
                rbRandom.Focus();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mvm.Stopping = true;
            mvm.cancelSource.Cancel();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) => mvm.ProcessReset();

        private void btnNewTable_Click(object sender, RoutedEventArgs e) => mvm.LoadAndShowNewTable(Window.GetWindow(this));

        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            SearchResults searchResults = new SearchResults();
            searchResults.Owner = this;
            searchResults.Show();
        }

        private void mnuDefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            GameSettings gameSettings = new();
            gameSettings.Owner = this;
            gameSettings.ShowDialog();
        }

        private void mnuGenerateSpinfile_Click(object sender, RoutedEventArgs e)
        {
            SpinfileGenerator spinfileGenerator = new();
            spinfileGenerator.Owner = this;
            spinfileGenerator.ShowDialog();
        }

        public void ReloadSettings()
        {
            mvm.ReloadSettings(SettingReload());
            mvm.ProcessReset();
        }

        private Setting SettingReload()
        {
            FileInfo fileInfo = new("settings.json");
            if (!fileInfo.Exists)
                return mvm.CleanDashboardSetting();
            using var streamer = new StreamReader(fileInfo.Open(FileMode.Open));
            Setting newSetting = JsonConvert.DeserializeObject<Setting>(streamer.ReadToEnd())!;
            if (newSetting == null)
                return mvm.CleanDashboardSetting();

            return newSetting;
        }

        public async void ReloadGameSettingBindings()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                BindingOperations.GetBindingExpression((DependencyObject)txtTables, TextBox.TextProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression((DependencyObject)txtRandom, TextBox.TextProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression((DependencyObject)txtRowLimit, TextBox.TextProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression((DependencyObject)txtCountLimit, TextBox.TextProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression((DependencyObject)txtGSLimit, TextBox.TextProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression((DependencyObject)txtR1W, TextBox.TextProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression((DependencyObject)txtTW, TextBox.TextProperty)?.UpdateTarget();
            });
        }

        private void rbRandom_Checked(object sender, RoutedEventArgs e) => mvm.LoadGameTypeSettings();

        private void rbAutoplay_Checked(object sender, RoutedEventArgs e) => mvm.LoadGameTypeSettings();

        private void mnuReworkTester_Click(object sender, RoutedEventArgs e)
        {
            //ReworkDash rework = new();
            //rework.Owner = this;
            //rework.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //rework.ShowDialog();

            NewDashboard dash = new()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dash.ShowDialog();
        }
    }
}

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
            if (await mvm.GetSelectedGameType() != GameType.None)
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


                mvm.StartSpins(await mvm.GetNewCancellationToken());                
            }
            else
            {
                AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
                AlertNotifications.DisplayAlertMessage("Please select a spin type");
                rbRandom.Focus();
            }
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e) => await Task.Run(() => mvm.StopSpins());

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
            GameSettings gameSettings = new GameSettings();
            gameSettings.Owner = this;
            gameSettings.ShowDialog();
        }

        private void mnuGenerateSpinfile_Click(object sender, RoutedEventArgs e)
        {
            SpinfileGenerator spinfileGenerator = new SpinfileGenerator();
            spinfileGenerator.Owner = this;
            spinfileGenerator.ShowDialog();
        }

        public void ReloadSettings()
        {
            FileInfo fileInfo = new FileInfo("settings.json");
            if (!fileInfo.Exists)
                return;
            Setting newSetting = JsonConvert.DeserializeObject<Setting>(new StreamReader(fileInfo.Open(FileMode.Open)).ReadToEnd())!;
            if (newSetting == null)
                return;
            mvm.ReloadSettings(newSetting);
        }

        private void rbRandom_Checked(object sender, RoutedEventArgs e) => mvm.LoadGameTypeSettings();

        private void rbAutoplay_Checked(object sender, RoutedEventArgs e) => mvm.LoadGameTypeSettings();

        private void mnuReworkTester_Click(object sender, RoutedEventArgs e)
        {
            ReworkDash rework = new();
            rework.Owner = this;
            rework.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            rework.ShowDialog();
        }
    }
}

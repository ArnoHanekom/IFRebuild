using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;
using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using Infinity.Engine;
using Infinity.Data.Models;
using Infinity.Data.Constants;
using Infinity.Roulette.LayoutModels;
using Infinity.Roulette.Controls;
using Infinity.Services.Interfaces;
using System.Windows.Threading;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
using Infinity.Engine.Services;

namespace Infinity.Roulette
{
    public partial class NewTable : Window, IObserver<Table>, IObserver<ISpinWatcherService>
    {
        private readonly NewTableViewModel ntvm = Container.container.Resolve<NewTableViewModel>();
        public RouletteGame Game { get; set; } = default!;
        public bool IsNewTable { get; set; }
        IDisposable unsub { get; set; } = default!;

        //public NewTable(IEngineService engineService, IOddWinService oddWinService)
        //{
        //    ntvm.GameTable = new Table(engineService, oddWinService);
        //    ntvm.TableGame = ntvm.GameTable.Game;
        //    NewTableStartUp();
        //}

        public NewTable(IEngineService engineService)
        {
            ntvm.GameTable = new Table(engineService);
            ntvm.TableGame = ntvm.GameTable.Game;
            NewTableStartUp();
        }

        public NewTable(Table selectedTable)
        {            
            ntvm.GameTable = selectedTable;
            ntvm.TableGame = ntvm.GameTable.Game;
            NewTableStartUp();
        }

        private void NewTableStartUp()
        {
            ntvm.LoadDefaults();
            Game = ntvm.GameTable.Game;
            unsub = ntvm.Subscribe(this);
            DataContext = ntvm;
            InitializeComponent();
        }

        private void ResolveViewModelContainer()
        {
            if (Container.container == null)
                return;
            
        }

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidNumber)
            {
                if (!ntvm.IsManual)
                {
                    ntvm.SetOtherCountStyle();
                }
                await Task.Run(delegate
                {
                    ntvm.StartSpin();
                });
            }
        }


        private async void btnReset_Click(object sender, RoutedEventArgs e) => await Task.Run(() => ntvm.ResetTable());

        private async void btnStop_Click(object sender, RoutedEventArgs e) => await Task.Run(() => ntvm.StopSpins());

        private void txtRandom_KeyDown(object sender, KeyEventArgs e)
        {
            var manText = txtManual.Text.Trim();
            if (manText != "")
            {
                ntvm.Manual = null;
                ntvm.IsManual = false;
            }   

            if (e.Key == Key.Return)
            {
                if (int.TryParse(txtRandom.Text, out int randomNumber))
                {
                    ntvm.Random = randomNumber;
                    btnPlay_Click(sender, e);
                }
            }
        }

        private void txtManual_KeyDown(object sender, KeyEventArgs e)
        {
            var rndText = txtRandom.Text.Trim();
            if (rndText != "")
                ntvm.Random = null;

            ntvm.IsManual = true;

            if (e.Key == Key.Return)
            {
                if (int.TryParse(txtManual.Text, out int manualNumber))
                {
                    ntvm.Manual = manualNumber;
                    btnPlay_Click(sender, e);
                }
            }
        }

        private bool IsValidNumber
        {
            get
            {
                if (ntvm.IsManual)
                {
                    if (ntvm.Manual.HasValue)
                    {
                        if (ntvm.Manual.Value > 0 && ntvm.Manual.Value < 37)
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                return true;
            }
        }

        public void OnCompleted()
        {
            if (ntvm.Spinning)
                return;
            UpdateLatestTableDetails();
        }

        private async void UpdateLatestTableDetails()
        {
            Data.Models.TableGameSetting setting = (await ntvm.GetCurrentSetting())!;
            await Dispatcher.InvokeAsync(() =>
            {
                Game = ntvm.GameTable.Game;
                ntvm.Manual = new int?();
                if (ntvm.IsReset)
                {
                    ntvm.Random = (setting?.RandomNumbers);
                    ntvm.RowLimit = (setting?.RowLimit);
                    ntvm.CountLimit = (setting?.CountLimit);
                    ntvm.GSLimit = (setting?.GSLimit);
                    ntvm.R1WLimit = (setting?.R1WLimit);
                    ntvm.TWLimit = (setting?.TWLimit);
                    UnSelectAutoplay();
                    SelectAutoplay((setting?.AutoplayNumber) ?? 10);
                }
                BetWindow.Game = ntvm.GameTable.Game;
                if (ntvm.IsManual)
                    txtManual.Focus();
                else
                    txtRandom.Focus();
                List<int> spinHistory = new List<int>(ntvm.GameTable.Game.SpinHistory.TakeLast(100));
                spinHistory.Reverse();
                ntvm.GameHistory = new List<Label>(new GameSpinHistory(spinHistory));
                ntvm.TotalGameSpins = ntvm.GameTable.Game.Spins;
                ntvm.GameRows = ntvm.GameTable.Game.GetRows();
                ntvm.GameCounts = ntvm.GameTable.Game.GetCounts();
                ntvm.GameGS = ntvm.GameTable.Game.GetGS();
                ntvm.GameMaxGS = ntvm.GameTable.Game.GetMaxGS();
                BindingOperations.GetBindingExpression(GameSpinHistoryList, ItemsControl.ItemsSourceProperty)?.UpdateTarget();
                DependencyProperty textProperty = TextBlock.TextProperty;
                BindingOperations.GetBindingExpression(tbTotalGameSpins, textProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression(tbGameRows, textProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression(tbGameCounts, textProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression(tbGameGS, textProperty)?.UpdateTarget();
                BindingOperations.GetBindingExpression(tbGameMaxGS, textProperty)?.UpdateTarget();
                ntvm.IsManual = false;
                ntvm.IsReset = false;
            });
        }

        private void UnSelectAutoplay()
        {
            foreach (ListBoxItem listBoxItem in cbAutoplays.Items)
                listBoxItem.IsSelected = false;
        }

        private void SelectAutoplay(int ap)
        {
            foreach (ComboBoxItem comboBoxItem in cbAutoplays.Items)
            {
                if (comboBoxItem.Content.ToString() == ap.ToString())
                    comboBoxItem.IsSelected = true;
            }
        }

        public void OnError(Exception error) => throw new NotImplementedException();

        public void OnNext(Table value)
        {
        }

        public void OnNext(ISpinWatcherService value) => throw new NotImplementedException();

        private async void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                ntvm.Spinfile = await ntvm.LoadSpinfileAsync(openFileDialog.FileName);
                await Task.Run(delegate
                {
                    ntvm.StartSpinfile();
                });
            }
        }

        private void mnuDefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            TableGameSetting tableGameSetting = new TableGameSetting();
            tableGameSetting.Owner = this;
            tableGameSetting.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            tableGameSetting.ShowDialog();
        }

        public void ReloadSettings()
        {
            FileInfo fileInfo = new FileInfo("table-settings.json");
            if (!fileInfo.Exists)
                return;
            TableSetting newSetting = JsonConvert.DeserializeObject<TableSetting>(new StreamReader(fileInfo.Open(FileMode.Open)).ReadToEnd())!;
            if (newSetting == null)
                return;
            ntvm.ReloadSettings(newSetting);
        }

    }
}

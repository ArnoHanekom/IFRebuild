using Infinity.Data.Models;
using Infinity.Roulette.Containers;
using Infinity.Roulette.Statics;
using Infinity.Roulette.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace Infinity.Roulette
{
    public partial class SearchResults : Window
    {
        private readonly SearchResultsViewModel searchVM = Container.container.Resolve<SearchResultsViewModel>();
        public double[]? OriginalGridColumnWidths { get; set; } = null;
        public double[]? InitialGridColumnWidths { get; set; } = null;
        private double[]? _saveColSizes { get; set; } = null;
        private void ResultsGrid_Loaded(object sender, RoutedEventArgs e) => SetupInitialWidths();
        public SearchResults()
        {
            DataContext = searchVM;
            InitializeComponent();
            SearchResultsTabs.FontFamily = Typography.CenturyGothic;
            AvailableResultsPage.FontFamily = Typography.CenturyGothic;
            OpenedResultsPage.FontFamily = Typography.CenturyGothic;
            ResultsGrid.FontFamily = Typography.CenturyGothic;
            OpenResultsGrid.FontFamily = Typography.CenturyGothic;
        }
        private void ResultsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is not Table table)
                return;
            e.Row.Style = Typography.NormalResultRow;
            if (!table.R1WMatch && !table.TWMatch)
                return;
            e.Row.Style = Typography.HighlightWinsMatch;
        }
        private void ResultsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            List<Table> tableList = [];
            if (dataGrid != null && dataGrid.SelectedItem is Table selectedItem)
            {
                NewTable newTable = new(selectedItem)
                {
                    Owner = GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                newTable.Show();
                newTable.Owner = null;
                tableList = dataGrid.Items.Cast<Table>().ToList();
                tableList.Remove(selectedItem);
                searchVM.AddToOpenResults(selectedItem);
            }
            ResultsGrid.ItemsSource = tableList;
        }
        private void SetupInitialWidths()
        {
            InitialGridColumnWidths ??= new double[11];
            OriginalGridColumnWidths ??= new double[11];
            for (int index = 0; index < ResultsGrid.Columns.Count; ++index)
            {
                InitialGridColumnWidths[index] = ResultsGrid.Columns[index].MinWidth;
                OriginalGridColumnWidths[index] = ResultsGrid.Columns[index].ActualWidth;
            }
            ResultsGrid.Width = ResultsWindow.Width - 25.0;
        }
        private async void cbRunSpinfileAll_Checked(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
            });
            await SelectAll(select: true);
        }

        private async Task<bool> CheckAllAsync()
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
            });
            await SelectAll(select: true);

            return true;
        }

        private async void cbRunSpinfileAll_Unchecked(object sender, RoutedEventArgs e) => await SelectAll(false);
        private async void cbRunSpinfileLimit_Checked(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileAll.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
            });
            await SelectAll(select: false);
            await SelectAllLimits(select: true);
        }
        private async void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => {
                ResultsGrid.Width = ResultsWindow.Width - 25.0;
            });
        }       
        private async void cbRunSpinfileLimit_Unchecked(object sender, RoutedEventArgs e) => await SelectAllLimits(false);
        private async void cbRunSpinfileR1W_Checked(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileAll.IsChecked = false;
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
            });
            await SelectAll(select: false);
            await SelectAllR1W(select: true);
        }
        private async void cbRunSpinfileTW_Checked(object sender, RoutedEventArgs e)
        {
            await base.Dispatcher.InvokeAsync(() => {
                cbRunSpinfileAll.IsChecked = false;
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
            });
            await SelectAll(select: false);
            await SelectAllTW(select: true);
        }
        private async void cbRunSpinfileTW_Unchecked(object sender, RoutedEventArgs e) => await SelectAllTW(false);
        private async void cbRunSpinfileR1W_Unchecked(object sender, RoutedEventArgs e) => await SelectAllR1W(false);
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var table = (Table)cb.DataContext;
            foreach (Table selected in ResultsGrid.Items.Cast<Table>())
            {
                if (selected.TableId == table.TableId && selected.Autoplay == table.Autoplay)
                {
                    selected.RunSpinfile = true;
                }
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var table = (Table)cb.DataContext;
            foreach (Table selected in ResultsGrid.Items.Cast<Table>())
            {
                if (selected.TableId == table.TableId && selected.Autoplay == table.Autoplay)
                {
                    selected.RunSpinfile = false;
                }
            }
        }

        private async Task ObsAllSelect(bool select)
        {
            await Task.Run(() =>
            {
                foreach (var item in searchVM.LoadedResults)
                {
                    item.RunSpinfile = select;
                }
            });
        }

        private async Task SelectAll(bool select) =>
            await Dispatcher.InvokeAsync(async () => ResultsGrid.ItemsSource = await allSelected(select));
        private async Task<Table[]> allSelected(bool isSelected) =>
            await Task.Run(() => getCheckList(ResultsGrid, CheckType.All, isSelected).ToArray());
        private async Task SelectAllR1W(bool select) =>
            await Dispatcher.InvokeAsync(async () => ResultsGrid.ItemsSource = await r1wAllSelected(select));
        private async Task<Table[]> r1wAllSelected(bool isSelected) =>
            await Task.Run(() => getCheckList(ResultsGrid, CheckType.R1W, isSelected).ToArray());
        private async Task SelectAllTW(bool select) =>
            await Dispatcher.InvokeAsync(async () => ResultsGrid.ItemsSource = await twAllSelected(select));
        private async Task<Table[]> twAllSelected(bool isSelected) =>
            await Task.Run(() => getCheckList(ResultsGrid, CheckType.TW, isSelected).ToArray());
        private async Task SelectAllLimits(bool select) =>
            await Dispatcher.InvokeAsync(async () => ResultsGrid.ItemsSource = await limitsAllSelected(select));
        private async Task<Table[]> limitsAllSelected(bool isSelected) =>
            await Task.Run(() => getCheckList(ResultsGrid, CheckType.Limits, isSelected).ToArray());

        private readonly Func<DataGrid, CheckType, bool, IEnumerable<Table>> getCheckList = (grid, ctype, select) =>
        {
            switch (ctype)
            {
                case CheckType.All:
                    return grid.Items.Cast<Table>().Select(t => {
                        t.RunSpinfile = select;
                        return t;
                    });
                case CheckType.R1W:
                    return grid.Items.Cast<Table>().Select(t => {
                        if (t.R1WMatch)
                            t.RunSpinfile = select;
                        return t;
                    });
                case CheckType.TW:
                    return grid.Items.Cast<Table>().Select(t => {
                        if (t.TWMatch)
                            t.RunSpinfile = select;
                        return t;
                    });
                case CheckType.Limits:
                    return grid.Items.Cast<Table>().Select(t => {
                        if (t.ExactMatch)
                            t.RunSpinfile = select;
                        return t;
                    });
                default:
                    return grid.Items.Cast<Table>().Select(t =>
                    {
                        t.RunSpinfile = select;
                        return t;
                    });
            }
        };

        private async void btnSpinFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            bool? dialog = openFileDialog.ShowDialog();
            if (!(dialog.GetValueOrDefault() & dialog.HasValue))
                return;
            await Task.Run(() => searchVM.LoadSpinfile(openFileDialog.FileName));
        }
        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            var spinfileTables = ResultsGrid.Items.Cast<Table>().Where(t => t.RunSpinfile);
            if (spinfileTables == null || !spinfileTables.Any())
                return;
            await Task.Run(() => searchVM.StartSpinfileSpins(spinfileTables, this));
        }

        public async void ReloadGrid()
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileAll.IsChecked = false;
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
                ResultsGrid.ItemsSource = searchVM.LoadedResults;
            });
        }
        private async void btnStop_Click(object sender, RoutedEventArgs e) => await Task.Run(searchVM.StopSpins);
        private async void cbR1Wonly_Checked(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileAll.IsChecked = false;
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
                ResultsGrid.ItemsSource = searchVM.LoadedResults.Where(lr => lr.R1WMatch);
            });
        }
        private async void cbR1Wonly_Unchecked(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() => {
                cbRunSpinfileAll.IsChecked = false;
                cbRunSpinfileLimit.IsChecked = false;
                cbRunSpinfileR1W.IsChecked = false;
                cbRunSpinfileTW.IsChecked = false;
                ResultsGrid.ItemsSource = searchVM.LoadedResults;
            });
        }
        private async void SearchResultsTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is not TabControl tabControl) return;
            TabItem activeTab = (TabItem)tabControl.SelectedItem;
            if (!(searchVM.GridSize > 0.0) || activeTab == null) return;

            if (activeTab.Name != "AvailableResultsPage")
            {
                if (activeTab.Name != "OpenedResultsPage") return;
                await Dispatcher.InvokeAsync(() => {
                    OpenResultsGrid.Width = searchVM.GridSize;
                    if (_saveColSizes != null)
                    {
                        for (int j = 0; j < OpenResultsGrid.Columns.Count; j++)
                        {
                            OpenResultsGrid.Columns[j].MinWidth = _saveColSizes[j];
                            OpenResultsGrid.Columns[j].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
                        }
                    }
                });
                return;
            }

            await Dispatcher.InvokeAsync(() => {
                if (OriginalGridColumnWidths != null)
                {
                    for (int i = 0; i < ResultsGrid.Columns.Count; i++)
                    {
                        ResultsGrid.Columns[i].MinWidth = OriginalGridColumnWidths[i] - 30.0;
                        ResultsGrid.Columns[i].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
                    }
                    double num = OriginalGridColumnWidths.Sum() - 300.0;
                    ResultsGrid.MinWidth = num;
                    ResultsGrid.Width = num;
                }
            });
        }

        private void OpenResultsGrid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OpenResultsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
        }

        private void ResultsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ResultsGrid_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            updateCounter++;
        }

        private void ResultsGrid_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            updateCounter++;

            if (updateCounter == gridItems) updateCounter = 0;
        }

        private int updateCounter { get; set; } = 0;
        private int gridItems => searchVM.LoadedResults.Count;
    }
}

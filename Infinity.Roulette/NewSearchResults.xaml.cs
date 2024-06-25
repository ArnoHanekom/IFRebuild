using Infinity.Data.Models;
using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace Infinity.Roulette;

public partial class NewSearchResults : Window
{
    private readonly SearchResultsViewModel searchVM = Container.container.Resolve<SearchResultsViewModel>();

    public double[] OriginalGridColumnWidths { get; set; } = default!;

    public double[] InitialGridColumnWidths { get; set; } = default!;

    private double[]? saveColSizes { get; set; }

    private double resultsWindowWidth { get; set; }
    public NewSearchResults()
    {
        DataContext = searchVM;
        InitializeComponent();
        SearchResultsTabs.FontFamily = new FontFamily("Century Gothic");
        AvailableResultsPage.FontFamily = new FontFamily("Century Gothic");
        OpenedResultsPage.FontFamily = new FontFamily("Century Gothic");
        ResultsGrid.FontFamily = new FontFamily("Century Gothic");
        OpenResultsGrid.FontFamily = new FontFamily("Century Gothic");
    }
    private void ResultsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        if (e.Row.Item is not Table table)
            return;
        e.Row.Style = Application.Current.FindResource("NormalResultRow") as Style;

        if (table.R1WMatch || table.TWMatch)
            e.Row.Style = Application.Current.FindResource("HighlightWinsMatch") as Style;
    }
    private void ResultsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        DataGrid dataGrid = (DataGrid)sender;
        List<Table> tableList = new List<Table>();
        if (dataGrid != null && dataGrid.SelectedItem is Table selectedItem)
        {
            NewTable newTable = new NewTable(selectedItem);
            newTable.Owner = GetWindow(this);
            newTable.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newTable.Show();
            newTable.Owner = null;
            tableList = dataGrid.Items.Cast<Table>().ToList();
            tableList.Remove(selectedItem);
            searchVM.AddToOpenResults(selectedItem);
        }
        ResultsGrid.ItemsSource = tableList;
    }
    private void ResultsGrid_Loaded(object sender, RoutedEventArgs e) => SetupInitialWidths();
    private void SetupInitialWidths()
    {
        double[] numArray;
        if (InitialGridColumnWidths == null)
            InitialGridColumnWidths = numArray = new double[11];
        if (OriginalGridColumnWidths == null)
            OriginalGridColumnWidths = numArray = new double[11];
        for (int index = 0; index < ResultsGrid.Columns.Count(); ++index)
        {
            InitialGridColumnWidths[index] = ResultsGrid.Columns[index].MinWidth;
            OriginalGridColumnWidths[index] = ResultsGrid.Columns[index].ActualWidth;
        }
        ResultsGrid.Width = ResultsWindow.Width - 25.0;
    }
    private void UpdateOwnerWidth()
    {
        if (Window.GetWindow(this) is not NewSearchResults window || window.Width >= InitialGridColumnWidths.Sum() + 125.0)
            return;
        window.Width = InitialGridColumnWidths.Sum() + 125.0;
    }
    private async void cbRunSpinfileAll_Checked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileLimit.IsChecked = false;
        });
        await SelectAll(select: true);
    }
    private async void cbRunSpinfileAll_Unchecked(object sender, RoutedEventArgs e) => await SelectAll(false);
    private async void cbRunSpinfileLimit_Checked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
        });
        await SelectAll(false);
        await SelectAllLimits(true);
    }
    private async void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            ResultsGrid.Width = ResultsWindow.Width - 25.0;
        });
    }
    private void ResizeGrid() => Dispatcher.Invoke(() =>
    {
        double num1 = Height - 65.0;
        double num2 = ResultsWindow.Width - 20.0;
        if (num1 > 100.0)
            ResultsGrid.MaxHeight = num1;
        ResultsGrid.Width = num2;
        for (int index = 0; index < ResultsGrid.Columns.Count(); ++index)
        {
            if (InitialGridColumnWidths != null)
                ResultsGrid.Columns[index].MinWidth = InitialGridColumnWidths[index];
            ResultsGrid.Columns[index].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
        }
        ResultsGrid.UpdateLayout();
    });
    private async void cbRunSpinfileLimit_Unchecked(object sender, RoutedEventArgs e) => await SelectAllLimits(false);
    private async void cbRunSpinfileR1W_Checked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            cbRunSpinfileLimit.IsChecked = false;
        });
        await SelectAll(select: false);
        await SelectAllR1W(select: true);
    }
    private async void cbRunSpinfileTW_Checked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            cbRunSpinfileLimit.IsChecked = false;
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
    private async Task SelectAll(bool select)
    {
        await Dispatcher.InvokeAsync(() => ResultsGrid.ItemsSource = SelectAllCheck(select));
    }

    private async Task SelectAllR1W(bool select)
    {
        NewSearchResults searchResults = this;
        IEnumerable<Table> itemsList = searchResults.ResultsGrid.Items.Cast<Table>().Select(t =>
        {
            if (t.R1WMatch)
                t.RunSpinfile = select;
            return t;
        });
        await searchResults.Dispatcher.InvokeAsync((Action)(() => ResultsGrid.ItemsSource = itemsList));
    }
    private async Task SelectAllTW(bool select)
    {
        NewSearchResults searchResults = this;
        IEnumerable<Table> itemsList = searchResults.ResultsGrid.Items.Cast<Table>().Select(t =>
        {
            if (t.TWMatch)
                t.RunSpinfile = select;
            return t;
        });
        await searchResults.Dispatcher.InvokeAsync((Action)(() => ResultsGrid.ItemsSource = itemsList));
    }
    private async Task SelectAllLimits(bool select)
    {        
        await Dispatcher.InvokeAsync(() => ResultsGrid.ItemsSource = SelectAllLimitsCheck(select));
    }
    private List<Table> SelectAllLimitsCheck(bool select)
    {
        List<Table> allResults = [.. ResultsGrid.Items.Cast<Table>()];
        foreach (var table in allResults.Where(t => t.ExactMatch))
        {
            table.RunSpinfile = select;
        }
        return allResults;
    }
    private List<Table> SelectAllCheck(bool select)
    {
        List<Table> allResults = [.. ResultsGrid.Items.Cast<Table>()];
        foreach (var table in allResults)
        {
            table.RunSpinfile = select;
        }
        return allResults;
    }
    private List<Table> SelectR1WCheck(bool select)
    {
        List<Table> allResults = [.. ResultsGrid.Items.Cast<Table>()];
        foreach (var table in allResults)
        {
            table.RunSpinfile = select;
        }
        return allResults;
    }

    private async void btnSpinFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        bool? nullable = openFileDialog.ShowDialog();
        bool flag = true;
        if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
            return;
        await Task.Run(() => searchVM.LoadSpinfile(openFileDialog.FileName));
    }
    private async void btnPlay_Click(object sender, RoutedEventArgs e)
    {
        List<Table> spinfileTables = GetSpinfileTables();
        if (spinfileTables == null || spinfileTables.Count <= 0)
            return;

        await Dispatcher.InvokeAsync(() =>
        {
            ResultsGrid.ItemsSource = null;
        });

        await searchVM.PrepareSpinStartAsync(spinfileTables.ToList()).ConfigureAwait(false);
        await searchVM.PlaySpinfileTablesAsync(this, searchVM.cancelToken).ConfigureAwait(false);
    }

    private List<Table> GetSpinfileTables()
    {
        return [.. ResultsGrid.Items.Cast<Table>().Where(t => t.RunSpinfile)];
    }

    public async void ReloadGrid()
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            cbRunSpinfileLimit.IsChecked = false;
            ResultsGrid.ItemsSource = searchVM.LoadedResults;
        });
    }
    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
        searchVM.Stopping = true;
        searchVM.cancelSource.Cancel();
    }
    private async void cbR1Wonly_Checked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            cbRunSpinfileLimit.IsChecked = false;
            ResultsGrid.ItemsSource = searchVM.LoadedResults.Where((Table lr) => lr.R1WMatch);
        });
    }
    private async void cbR1Wonly_Unchecked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            cbRunSpinfileLimit.IsChecked = false;
            ResultsGrid.ItemsSource = searchVM.LoadedResults;
        });
    }
    private async void SearchResultsTabs_SelectionChanged(
      object sender,
      SelectionChangedEventArgs e)
    {
        if (e.Source is not TabControl tabControl)
        {
            return;
        }
        TabItem activeTab = (TabItem)tabControl.SelectedItem;
        if (!(searchVM.GridSize > 0.0) || activeTab == null)
        {
            return;
        }
        string name = activeTab.Name;
        if (!(name == "AvailableResultsPage"))
        {
            if (!(name == "OpenedResultsPage"))
            {
                return;
            }
            await base.Dispatcher.InvokeAsync(delegate
            {
                OpenResultsGrid.Width = searchVM.GridSize;
                if (saveColSizes != null)
                {
                    for (int j = 0; j < OpenResultsGrid.Columns.Count(); j++)
                    {
                        OpenResultsGrid.Columns[j].MinWidth = saveColSizes[j];
                        OpenResultsGrid.Columns[j].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
                    }
                }
            });
            return;
        }
        await base.Dispatcher.InvokeAsync(delegate
        {
            if (OriginalGridColumnWidths != null)
            {
                for (int i = 0; i < ResultsGrid.Columns.Count(); i++)
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
}
﻿using Infinity.Data.Models;
using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using Microsoft.Win32;
using ModernWpf.Controls;
using System.Diagnostics;
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
        searchVM.IsLoadingEvent = true;
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
    private void ResultsGrid_Loaded(object sender, RoutedEventArgs e)
    {
        SetupInitialWidths();
        searchVM.IsLoadingEvent = false;
    }
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
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = true;
        });
        await Dispatcher.InvokeAsync(() =>
        {
            //cbRunSpinfileLimit.IsChecked = false;
        });
        await SelectAll(true);
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = false;
        });
    }
    private async void cbRunSpinfileAll_Unchecked(object sender, RoutedEventArgs e) => await SelectAll(false);
    private async void cbRunSpinfileLimit_Checked(object sender, RoutedEventArgs e)
    {
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = true;
        });
        await Dispatcher.InvokeAsync(() =>
        {
            cbRunSpinfileAll.IsChecked = false;
        });
        await SelectAll(false);
        await SelectAllLimits(true);
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = false;
        });
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
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = true;
        });
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            //cbRunSpinfileLimit.IsChecked = false;
        });
        await SelectAll(false);
        await SelectAllR1W(true);
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = false;
        });
    }
    private async void cbRunSpinfileTW_Checked(object sender, RoutedEventArgs e)
    {
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = true;
        });
        await Dispatcher.InvokeAsync(() =>
        {
            cbRunSpinfileAll.IsChecked = false;
            //cbRunSpinfileLimit.IsChecked = false;
        });
        await SelectAll(select: false);
        await SelectAllTW(select: true);
        await Task.Run(() =>
        {
            searchVM.IsLoadingEvent = false;
        });
    }
    private async void cbRunSpinfileTW_Unchecked(object sender, RoutedEventArgs e) => await SelectAllTW(false);
    private async void cbRunSpinfileR1W_Unchecked(object sender, RoutedEventArgs e) => await SelectAllR1W(false);
    private async void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            var cb = (CheckBox)sender;
            var table = (Table)cb.DataContext;
            await SelectCheckbox(true, table.TableId, table.Autoplay);
        }
        catch { }
    }
    private async Task SelectCheckbox(bool select, int tableId, int autoplay)
    {
        await Dispatcher.InvokeAsync(() => ResultsGrid.ItemsSource = SelectCheckboxCheck(select, tableId, autoplay));
    }
    private List<Table> SelectCheckboxCheck(bool select, int tableId, int autoplay)
    {
        List<Table> allResults = [.. ResultsGrid.Items.Cast<Table>()];
        foreach (var table in allResults.Where(t => t.TableId == tableId && t.Autoplay == autoplay))
        {
            table.RunSpinfile = select;
        }
        return allResults;
    }

    private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            var cb = (CheckBox)sender;
            var table = (Table)cb.DataContext;
            await SelectCheckbox(false, table.TableId, table.Autoplay);
        }
        catch { }
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

        if (spinfileTables.Count == 0) spinfileTables = _manualSelected;

        await Dispatcher.InvokeAsync(() =>
        {
            ResultsGrid.ItemsSource = null;
        });
        _manualSelected = [];
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
            //cbRunSpinfileLimit.IsChecked = false;
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
            //cbRunSpinfileLimit.IsChecked = false;
            ResultsGrid.ItemsSource = searchVM.LoadedResults.Where((Table lr) => lr.R1WMatch);
        });
    }
    private async void cbR1Wonly_Unchecked(object sender, RoutedEventArgs e)
    {
        await base.Dispatcher.InvokeAsync(delegate
        {
            cbRunSpinfileAll.IsChecked = false;
            //cbRunSpinfileLimit.IsChecked = false;
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

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        var toggler = sender as ToggleSwitch;
        if (toggler is null) return;
        var table = toggler.DataContext as Table;
        if (table is null) return;
        table.RunSpinfile = true;
        _manualSelected.Add(table);
    }
    private List<Table> _manualSelected { get; set; } = [];

    private List<Table> UncheckRunWithSpinfile()
    {
        return ResultsGrid.Items.Cast<Table>().Select(i =>
            {
                i.RunSpinfile = false;
                return i;
            }).ToList();
    }

    private async void cbSpinfileCounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var uncheckedList = UncheckRunWithSpinfile();
            if (searchVM.SelectedSpinfileCount == -2) ResultsGrid.ItemsSource = uncheckedList;
            if (searchVM.SelectedSpinfileCount != -2) ResultsGrid.ItemsSource = CheckSelectedCountRunWithSpinfile(uncheckedList);
        });
    }

    private List<Table> CheckSelectedCountRunWithSpinfile(List<Table> source)
    {
        return source.Select(s =>
            {
                if (s.Counts == searchVM.SelectedSpinfileCount || searchVM.SelectedSpinfileCount == -1) s.RunSpinfile = true;
                return s;
            }).ToList();
    }

    private async void cbSpinfileRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var uncheckedList = UncheckRunWithSpinfile();
            if (searchVM.SelectedSpinfileRow == -2) ResultsGrid.ItemsSource = uncheckedList;
            if (searchVM.SelectedSpinfileRow != -2) ResultsGrid.ItemsSource = CheckSelectedRowRunWithSpinfile(uncheckedList);
        });
    }

    private List<Table> CheckSelectedRowRunWithSpinfile(List<Table> source)
    {
        return source.Select(s =>
        {
            if (s.Rows == searchVM.SelectedSpinfileRow || searchVM.SelectedSpinfileRow == -1) s.RunSpinfile = true;
            return s;
        }).ToList();
    }

    private async void cbSpinfileR1W_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var uncheckedList = UncheckRunWithSpinfile();
            if (searchVM.SelectedSpinfileR1W == -2) ResultsGrid.ItemsSource = uncheckedList;
            if (searchVM.SelectedSpinfileR1W != -2) ResultsGrid.ItemsSource = CheckSelectedR1WRunWithSpinfile(uncheckedList);
        });
    }

    private List<Table> CheckSelectedR1WRunWithSpinfile(List<Table> source)
    {
        return source.Select(s =>
        {
            if (s.FirstRowWin == searchVM.SelectedSpinfileR1W || searchVM.SelectedSpinfileR1W == -1) s.RunSpinfile = true;
            return s;
        }).ToList();
    }

    private async void cbSpinfileMaxGs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var uncheckedList = UncheckRunWithSpinfile();
            if (searchVM.SelectedSpinfileMaxGS == -2) ResultsGrid.ItemsSource = uncheckedList;
            if (searchVM.SelectedSpinfileMaxGS != -2) ResultsGrid.ItemsSource = CheckSelectedMaxGSRunWithSpinfile(uncheckedList);
        });
    }

    private List<Table> CheckSelectedMaxGSRunWithSpinfile(List<Table> source)
    {
        return source.Select(s =>
        {
            if (s.MaxGS == searchVM.SelectedSpinfileMaxGS || searchVM.SelectedSpinfileMaxGS == -1) s.RunSpinfile = true;
            return s;
        }).ToList();
    }
}
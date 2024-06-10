using Infinity.Roulette.Containers;
using Infinity.Roulette.Rework.Data.Models;
using Infinity.Roulette.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Infinity.Roulette;

public partial class ReworkDash : Window
{
    private readonly ReworkViewModel _reworkVM = default!;
    //private IProgress<double> prepareReporter { get; set; } = default!;
    //private IProgress<double> progressReporter { get; set; } = default!;

    public ReworkDash()
    {
        if (Container.container != null)
            _reworkVM = Container.container.Resolve<ReworkViewModel>();

        DataContext = _reworkVM;
        InitializeComponent();

        TypeSelector.RbCheckedHandler += TypeSelector_RbCheckedHandler;
    }

    private async void TypeSelector_RbCheckedHandler(object sender, RoutedEventArgs e)
    {

        await Dispatcher.InvokeAsync(() =>
        {
            _reworkVM.GetGameTypeSettings();
        });

        if (TypeSelector.RbChecked == 1)
        {
            await TableSettings.ReloadAutoplaysAsync(_reworkVM.Autoplays);
        }

        if (TypeSelector.RbChecked == 1) await TableSettings.ShowHideAutoplaySelectorAsync();
        if (TypeSelector.RbChecked == 0) await TableSettings.ShowHideAutoplaySelectorAsync(false);
        

        
    }

    private async void btnResults_Click_11(object sender, RoutedEventArgs e)
    {
        CancellationTokenSource cts = new();
        await _reworkVM.PrintHistoryAsync(cts.Token);
    }   
    private async void btnPager_Click(object sender, RoutedEventArgs e)
    {
        CancellationTokenSource cts = new();
        var pageBtn = sender as Button;
        int.TryParse(pageBtn?.CommandParameter.ToString(), out int selectedPage);
        await ChangePageAsync(selectedPage, cts.Token);

        var results = await _reworkVM.LoadResultForPageAsync(selectedPage, _reworkVM.PageSize, cts.Token);
        if (results.ResultItems.Length > 0)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                //dgResults.ItemsSource = results.ResultItems;
            });
        }
    }
    private async Task ChangePageAsync(int selectedPage, CancellationToken ct = default)
    {
        await _reworkVM.ChangePageAsync(selectedPage, ct);
    }
    private async void btnRunSpinfile_Click(object sender, RoutedEventArgs e)
    {
        
        var spinNumbers = BuildSpinfileList(await GetSpinfileAsync());
                        
        _reworkVM.SpinType = SpinType.Spinfile;

        await _reworkVM.ProcessSpinfileSpinsAsync(spinNumbers);
    }
    private async Task<string> GetSpinfileAsync(CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "";
            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
            }

            return fileName;
        });
    }
    private List<int> BuildSpinfileList(string filename)
    {
        var Spinfile = new List<int>();
        using (FileStream fileStream = File.Open(filename, FileMode.Open))
        {
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                while (!streamReader.EndOfStream)
                {
                    string str = streamReader.ReadLine()!;
                    if (!string.IsNullOrEmpty(str))
                    {
                        foreach (string s in str.Split('\t'))
                        {
                            int result;
                            if (int.TryParse(s, out result) && result > 0)
                                Spinfile.Add(result);
                        }
                    }
                }
            }
        }

        return Spinfile;
    }
    private async void btnRun_Click(object sender, RoutedEventArgs e)
    {
        await _reworkVM.GetNewCancellationAsync();
        var token = _reworkVM.CancellationSource.Token;
        await Dispatcher.InvokeAsync(() =>
        {
            _reworkVM.ShowRunBtn = false;
            _reworkVM.ShowStopBtn = true;
        });

        IProgress<double> prepareReporter = new Progress<double>(prepProg =>
        {
            pbLoadTablesUpdate.Value = prepProg;
        });

        IProgress<double> progressReporter = new Progress<double>(prog =>
        {
            pbProgressUpdate.Value = prog;
        });
        _reworkVM.PrepareReporter = prepareReporter;
        _reworkVM.ProgressReporter = progressReporter;

        await _reworkVM.SetReportersAsync(token);
        await Dispatcher.InvokeAsync(() =>
        {
            spDashboardSearchUpdate.Visibility = Visibility.Collapsed;
            spPrepareTablesUpdate.Visibility = Visibility.Visible;            
        });
        
        await _reworkVM.PreparePlaysAsync(token);
        await Dispatcher.InvokeAsync(() =>
        {
            spPrepareTablesUpdate.Visibility = Visibility.Collapsed;
            spDashboardSearchUpdate.Visibility = Visibility.Visible;
        });

        await _reworkVM.StartDashboardSearchAsync(token);

        await Task.Delay(500);
        await Dispatcher.InvokeAsync(() =>
        {
            spPrepareTablesUpdate.Visibility = Visibility.Collapsed;
            spDashboardSearchUpdate.Visibility = Visibility.Collapsed;
            _reworkVM.ShowStopBtn = false;
            _reworkVM.ShowRunBtn = true;
        });
    }
    private async void btnStop_Click(object sender, RoutedEventArgs e)
    {
        _reworkVM.CancellationSource.Cancel();

        while (spPrepareTablesUpdate.Visibility == Visibility.Visible || spDashboardSearchUpdate.Visibility == Visibility.Visible)
        {
            await Task.Delay(100);
        }
        await Task.Delay(300);
        await Dispatcher.InvokeAsync(() =>
        {
            _reworkVM.ShowStopBtn = false;
            _reworkVM.ShowRunBtn = true;
        });
    }
    private async void btnResults_Click(object sender, RoutedEventArgs e)
    {
        ReworkResults resultsWindow = new();
        resultsWindow.Owner = this;
        resultsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;        
        await resultsWindow.LoadGridResultsAsync();
        resultsWindow.ShowDialog();
        resultsWindow.Owner = null;
    }
    private async void btnReset_Click(object sender, RoutedEventArgs e)
    {
        await _reworkVM.ResetDashboardAsync();
        await Task.Delay(200);
        TableSettings.SetFocusToTables();
    }
    private void mnuDefaultSettings_Click(object sender, RoutedEventArgs e)
    {
        ReworkDefaultSettings reworkDefaultSettings = new();
        reworkDefaultSettings.Owner = this;
        reworkDefaultSettings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        reworkDefaultSettings.ShowDialog();
        reworkDefaultSettings.Owner = null;
    }
    public async Task ReloadSettingsAsync()
    {
        await Dispatcher.InvokeAsync(() =>
        {
            _reworkVM.GetGameTypeSettings();
        });
    }
}

public class Pager
{
    public int PageNumber { get; set; } = 1;
    public bool IsSelected { get; set; } = false;
    public bool IsEnabled => !IsSelected;
}
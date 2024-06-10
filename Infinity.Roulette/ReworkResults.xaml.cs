using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Infinity.Roulette;

public partial class ReworkResults : Window
{
    private readonly ReworkResultsViewModel vm = default!;
    public ReworkResults()
    {
        if (Container.container != null)
            vm = Container.container.Resolve<ReworkResultsViewModel>();

        DataContext = vm;
        InitializeComponent();        
    }

    public async Task LoadGridResultsAsync(CancellationToken ct = default)
    {
        await vm.LoadResultsAsync(ct);
        await vm.LoadOrderedResultsForDisplayAsync(ct);
        await ReloadGridSourceAsync();
    }

    private async void cbRunSpinfileAll_Click(object sender, RoutedEventArgs e)
    {
        var checkAll = sender as CheckBox;
        if (checkAll!.IsChecked.HasValue)
        {
            if (checkAll.IsChecked.Value)
            {
                await vm.LoadCheckAllListAsync();
                await ReloadGridSourceAsync();
            }
            else
            {
                await vm.LoadUnCheckAllListAsync();
                await ReloadGridSourceAsync();
            }
        }
    }

    private async void cbRunSpinfileLimit_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void cbRunSpinfileR1W_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void cbRunSpinfileTW_Click(object sender, RoutedEventArgs e)
    {

    }

    private async Task ReloadGridSourceAsync(CancellationToken ct = default)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            dgSearchResults.ItemsSource = vm.GridResults;
        });
    }
}
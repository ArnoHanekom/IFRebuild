using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Infinity.Roulette.Controls;

public partial class DashboardTableSettingsControl : UserControl
{
    public DashboardTableSettingsControl()
    {
        InitializeComponent();
        this.Loaded += DashboardTableSettingsControl_Loaded;
    }

    private void DashboardTableSettingsControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        tbTables.Focus();
    }

    public void SetFocusToTables()
    {
        tbTables.Focus();
    }

    public async Task ShowHideAutoplaySelectorAsync(bool show = true, CancellationToken ct = default)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var visible = show ? Visibility.Visible : Visibility.Collapsed;
            lblSelectAutoplays.Visibility = visible;
            cbSelectAutoplays.Visibility = visible;
        });
    }

    public async Task ReloadAutoplaysAsync(IEnumerable<ComboBoxItem> newAutoplays)
    {
        await Dispatcher.InvokeAsync(() => 
        {
            cbSelectAutoplays.ItemsSource = newAutoplays;
        });
    }
}
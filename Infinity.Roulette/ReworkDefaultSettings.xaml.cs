using Infinity.Data.Constants;
using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Infinity.Roulette;

public partial class ReworkDefaultSettings : Window
{

    private readonly ReworkDefaultSettingsViewModel vm = default!;
    public ReworkDefaultSettings()
    {
        if (Container.container != null)
            vm = Container.container.Resolve<ReworkDefaultSettingsViewModel>();
        DataContext = vm;
        InitializeComponent();
    }

    private async void cbGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cbox = sender as ComboBox;
        if (cbox is not null)
        {
            if (TableSettings is not null)
            {
                var val = ((ComboBoxItem)cbox.SelectedItem).Content.ToString();
                bool display = val!.ToLower() == "autoplay" ? true : false;
                vm.RouletteGameType = display ? GameType.Autoplay : GameType.Random;
                int changeTo = vm.RouletteGameType == GameType.Autoplay ? 1 : 0;

                await vm.ChangeCurrentSettingAsync(changeTo);
                vm.LoadSelectionSettings(val);
                
                await TableSettings.ShowHideAutoplaySelectorAsync(display);
            }            
        }
    }

    private async void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        await vm.DiscardAsync();
        Close();
    }

    private async void btnSave_Click(object sender, RoutedEventArgs e)
    {
        await vm.SaveSettingAsync();
    }
}
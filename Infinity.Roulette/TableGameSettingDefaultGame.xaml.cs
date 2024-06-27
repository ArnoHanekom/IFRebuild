using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System.Windows;
using Unity;

namespace Infinity.Roulette;

public partial class TableGameSettingDefaultGame : Window
{
    private TableGameSettingDefaultGameViewModel gsDefaultGameTypeVM { get; set; } = null!;
    public TableGameSettingDefaultGame()
    {
        if (Container.container != null)
            gsDefaultGameTypeVM = Container.container.Resolve<TableGameSettingDefaultGameViewModel>();
        DataContext = gsDefaultGameTypeVM;
        InitializeComponent();
    }
    private void btnSaveDefaultGame_Click(object sender, RoutedEventArgs e)
    {
        gsDefaultGameTypeVM.SaveDefaultGameSetting();
        ((TableGameSetting)Owner).UpdateDefaultGameType();
        Close();
    }
    private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
}
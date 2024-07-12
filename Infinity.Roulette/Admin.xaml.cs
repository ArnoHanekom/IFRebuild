using Infinity.Roulette.Containers;
using Infinity.Roulette.Statics;
using Infinity.Roulette.ViewModels;
using System.Windows;
using Unity;

namespace Infinity.Roulette;

public partial class Admin : Window
{
    private AdminViewModel adminVM { get; set; } = default!;
    public Admin()
    {

        if (Container.container != null)
            adminVM = Container.container.Resolve<AdminViewModel>();

        DataContext = adminVM;
        InitializeComponent();
    }

    private async void btnGenerateLicense_Click(object sender, RoutedEventArgs e)
    {
        await adminVM.GenerateNewLicenseAsync();
        AlertNotifications.PlayAlert("C:\\Windows\\Media\\Windows Message Nudge.wav");
        AlertNotifications.DisplayAlertMessage("New License Generated");
    }

    private async void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            if (txtUName.Text.Equals("admin", StringComparison.CurrentCultureIgnoreCase) && txtPWord.Password.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
            {
                tbError.Visibility = Visibility.Collapsed;
                gridAuthentication.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbError.Visibility = Visibility.Visible;
            }
        });
    }
}
using Infinity.Roulette.Containers;
using System.Windows;
using Unity;

namespace Infinity.Roulette
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Container.RegisterServices();
            if (Container.container != null)
                Container.container.Resolve<AppSettings>().LoadSettings();
            new Main().Show();
        }
    }
}

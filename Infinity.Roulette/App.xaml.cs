using DocumentFormat.OpenXml.Wordprocessing;
using Infinity.Roulette.Containers;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Unity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Infinity.Roulette.Commands;

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

    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
                services.AddSingleton<IAddObsObjects, AddObsObjects>();
            }).Build();
        }
    }
}

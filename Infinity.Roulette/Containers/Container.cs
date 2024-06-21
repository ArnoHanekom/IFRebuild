//using Infinity.Engine.Services;
using Infinity.Engine.Services;
using Infinity.Roulette.Rework.Services;
using Infinity.Roulette.Rework.Services.Impl;
using Infinity.Services.Interfaces;
using Infinity.Services.Services;
using Unity;
using Unity.Lifetime;

namespace Infinity.Roulette.Containers
{
    public static class Container
    {
        public static IUnityContainer container { get; set; } = new UnityContainer();
        public static void RegisterServices() => _registerServices();
        private static void _registerServices()
        {
            container.RegisterType<IGameSettingService, GameSettingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILimitService, LimitService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISettingService, SettingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITableSettingService, TableSettingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameTypeService, GameTypeService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDashGameService, DashGameService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISearchService, SearchService>(new ContainerControlledLifetimeManager());
            container.RegisterType<INumberGenerator, NumberGenerator>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITableService, TableService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpinWatcherService, SpinWatcherService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICountLabelService, CountLabelService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICountColorService, CountColorService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEngineService, EngineService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPlayService, PlayService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IReworkSettingService, ReworkSettingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConcurrentSearchService, ConcurrentSearchService>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IOddWinService, OddWinService>(new ContainerControlledLifetimeManager());
        }
    }
}

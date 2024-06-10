using Infinity.Engine;
using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Infinity.Roulette.Controls
{
    public partial class BettingWindow : UserControl
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), typeof(RouletteGame), typeof(BettingWindow), new PropertyMetadata(null, new PropertyChangedCallback(GamePropertyChanged)));

        private readonly BettingWindowViewModel bettingVM = Container.container.Resolve<BettingWindowViewModel>();

        public RouletteGame Game
        {
            get => (RouletteGame)GetValue(GameProperty);
            set
            {
                SetValue(GameProperty, value);
                bettingVM.PrepareTableLayout();
            }
        }

        private static void GamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not BettingWindow bettingWindow)
                return;
            bettingWindow.OnGamePropertyChanged(e);
        }

        private void OnGamePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            bettingVM.Game = (RouletteGame)e.NewValue;
            bettingVM.PrepareTableLayout();
        }

        public BettingWindow()
        {            
            DataContext = bettingVM;
            InitializeComponent();
        }
    }
}

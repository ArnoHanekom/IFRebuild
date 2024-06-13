using System.Windows;
using System.Windows.Threading;

namespace Infinity.Roulette.Statics
{
    public static class ExtensionHelper
    {
        private static readonly Action EmptyAction = delegate { };
        public static void RefreshControl(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyAction);
        }
    }
}

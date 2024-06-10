using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Infinity.Roulette.Controls
{
    /// <summary>
    /// Interaction logic for DashboardSpinTypeSelectorControl.xaml
    /// </summary>
    public partial class DashboardSpinTypeSelectorControl : UserControl
    {
        public event RoutedEventHandler? RbCheckedHandler;
        public int RbChecked = 0;

        public DashboardSpinTypeSelectorControl()
        {
            InitializeComponent();
        }

        private void rbRandom_Checked(object sender, RoutedEventArgs e)
        {
            RbChecked = 0;
            if (RbCheckedHandler is not null) RbCheckedHandler(this, e);
        }

        private void rbAutoplay_Checked(object sender, RoutedEventArgs e)
        {
            RbChecked = 1;
            if (RbCheckedHandler is not null) RbCheckedHandler(this, e);
        }
    }
}

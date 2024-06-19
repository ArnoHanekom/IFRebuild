using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.ViewModels
{
    public class NewDashboardViewModel : ViewModelBase
    {
        private bool _isPlaying { get; set; }
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
    }
}

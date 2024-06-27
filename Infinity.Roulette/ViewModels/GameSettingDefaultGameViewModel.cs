using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Infinity.Roulette.ViewModels
{
    public class GameSettingDefaultGameViewModel : ViewModelBase
    {
        private readonly ISettingService _settingService;
        private readonly IGameTypeService _gameTypeService;
        public GameSettingDefaultGameViewModel(ISettingService settingService, IGameTypeService gameTypeService)
        {
            _settingService = settingService;
            _gameTypeService = gameTypeService;
            _setting = _settingService.Get();
            LoadDefault();
        }
        private ObservableCollection<ComboBoxItem> _gameTypeOptions { get; set; } = null!;
        public ObservableCollection<ComboBoxItem> GameTypeOptions
        {
            get => _gameTypeOptions;
            set
            {
                if (_gameTypeOptions != value)
                    _gameTypeOptions = value;
                OnPropertyChanged(nameof(GameTypeOptions));
            }
        }
        private ComboBoxItem _selectedType { get; set; } = null!;
        public ComboBoxItem SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                    _selectedType = value;
                //if (_selectedType.Content.ToString() != Setting.DefaultGameType.ToString())
                //  Setting.DefaultGameType = _gameTypeService.Get(SelectedType.Content.ToString());
                OnPropertyChanged(nameof(SelectedType));
            }
        }
        private Setting _setting { get; set; }
        public Setting Setting
        {
            get => _setting;
            set
            {
                if (_setting != value)
                    _setting = value;
                OnPropertyChanged(nameof(Setting));
            }
        }
        public void LoadDefault()
        {
            List<ComboBoxItem> options = new List<ComboBoxItem>();
            _gameTypeService.GetTypes().ForEach(gt =>
            {
                List<ComboBoxItem> comboBoxItemList = options;
                comboBoxItemList.Add(new ComboBoxItem()
                {
                    Content = gt.ToString()
                });
            });
            GameTypeOptions = new ObservableCollection<ComboBoxItem>(options);
            SelectedType = GameTypeOptions.FirstOrDefault(o => o.Content.ToString() == Setting.DefaultGameType.ToString());
        }
        public void SaveDefaultGameSetting() => _settingService.Save(Setting);
    }
}

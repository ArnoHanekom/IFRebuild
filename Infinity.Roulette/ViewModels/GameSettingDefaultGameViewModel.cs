// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.GameSettingDefaultGameViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;


#nullable enable
namespace Infinity.Roulette.ViewModels
{
  public class GameSettingDefaultGameViewModel : ViewModelBase
  {
    private readonly ISettingService _settingService;
    private readonly IGameTypeService _gameTypeService;

    public GameSettingDefaultGameViewModel(
      ISettingService settingService,
      IGameTypeService gameTypeService)
    {
      _settingService = settingService;
      _gameTypeService = gameTypeService;
      _setting = _settingService.Get();
      LoadDefault();
    }

    private ObservableCollection<ComboBoxItem> _gameTypeOptions { get; set; }

    public ObservableCollection<ComboBoxItem> GameTypeOptions
    {
      get => _gameTypeOptions;
      set
      {
        if (_gameTypeOptions != value)
          _gameTypeOptions = value;
        OnPropertyChanged(nameof (GameTypeOptions));
      }
    }

    private ComboBoxItem _selectedType { get; set; }

    public ComboBoxItem SelectedType
    {
      get => _selectedType;
      set
      {
        if (_selectedType != value)
          _selectedType = value;
        if (_selectedType.Content.ToString() != Setting.DefaultGameType.ToString())
          Setting.DefaultGameType = _gameTypeService.Get(SelectedType.Content.ToString());
        OnPropertyChanged(nameof (SelectedType));
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
        OnPropertyChanged(nameof (Setting));
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
      SelectedType = GameTypeOptions.FirstOrDefault(o => o.Content.ToString() == _setting.DefaultGameType.ToString());
    }

    public void SaveDefaultGameSetting() => _settingService.Save(Setting);
  }
}

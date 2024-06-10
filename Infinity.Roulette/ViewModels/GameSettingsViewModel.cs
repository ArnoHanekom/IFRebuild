// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.GameSettingsViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;


#nullable enable
namespace Infinity.Roulette.ViewModels
{
  public class GameSettingsViewModel : ViewModelBase
  {
    private readonly ISettingService _settingService;
    private readonly IGameTypeService _gameTypeService;

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

    private ComboBoxItem _selectedType { get; set; } = new ComboBoxItem();

    public ComboBoxItem SelectedType
    {
      get => _selectedType;
      set
      {
        if (_selectedType != value)
          _selectedType = value;
        OnPropertyChanged(nameof (SelectedType));
        AutoplaySelected = _gameTypeService.Get(_selectedType.Content.ToString()) == GameType.Autoplay;
      }
    }

    private bool _autoplaySelected { get; set; }

    public bool AutoplaySelected
    {
      get => _autoplaySelected;
      set
      {
        if (_autoplaySelected != value)
          _autoplaySelected = value;
        if (!_autoplaySelected)
          SelectedAutoplayValue = 1;
        OnPropertyChanged(nameof (AutoplaySelected));
      }
    }

    private GameSetting _gameSetting { get; set; }

    public GameSetting GameSetting
    {
      get => _gameSetting;
      set
      {
        if (_gameSetting != value)
          _gameSetting = value;
        OnPropertyChanged(nameof (GameSetting));
      }
    }

    private IEnumerable<ComboBoxItem> _AutoplayOptions { get; set; }

    public IEnumerable<ComboBoxItem> AutoplayOptions
    {
      get => _AutoplayOptions;
      set
      {
        if (_AutoplayOptions != value)
          _AutoplayOptions = value;
        OnPropertyChanged(nameof (AutoplayOptions));
      }
    }

    private ComboBoxItem _SelectedAutoplay { get; set; }

    public ComboBoxItem SelectedAutoplay
    {
      get => _SelectedAutoplay;
      set
      {
        if (_SelectedAutoplay != value)
          _SelectedAutoplay = value;
        int result;
        if (int.TryParse(_SelectedAutoplay.Content.ToString(), out result))
          SelectedAutoplayValue = result;
        OnPropertyChanged(nameof (SelectedAutoplay));
      }
    }

    private int _SelectedAutoplayValue { get; set; }

    public int SelectedAutoplayValue
    {
      get => _SelectedAutoplayValue;
      set
      {
        if (_SelectedAutoplayValue != value)
          _SelectedAutoplayValue = value;
        OnPropertyChanged(nameof (SelectedAutoplayValue));
      }
    }

    public GameSettingsViewModel(ISettingService settingService, IGameTypeService gameTypeService)
    {
      _settingService = settingService;
      _gameTypeService = gameTypeService;
      LoadDefault();
    }

    private void LoadDefault()
    {
      GetLatestSetting();
      GameTypeOptions = new ObservableCollection<ComboBoxItem>(_gameTypeService.GetTypes().Where(gt => gt != GameType.None && gt != GameType.Manual && gt != GameType.Spinfile).Select(gt =>
      {
          return new ComboBoxItem()
          {
              Content = gt.ToString()
          };
      }));
      if (GameTypeOptions.Count() > 0)
        SelectedType = GameTypeOptions[0];
      PrepareAutoplayOptions();
    }

    private void PrepareAutoplayOptions()
    {
      List<int> source = new List<int>();
      for (int index = 10; index <= 100; index += 10)
        source.Add(index);
      for (int index = 120; index <= 200; index += 20)
        source.Add(index);
      for (int index = 250; index <= 300; index += 50)
        source.Add(index);
      AutoplayOptions = source.Select(c =>
      {
          if (c == 10)
          {
              SelectedAutoplay = new ComboBoxItem()
              {
                  Content = c.ToString(),
                  FontFamily = new FontFamily("Century Gothic"),
                  IsSelected = true
              };
              return SelectedAutoplay;
          }
          return new ComboBoxItem()
          {
              Content = c.ToString(),
              FontFamily = new FontFamily("Century Gothic")
          };
      });
    }

    public void GetLatestSetting() => Setting = _settingService.Get();

    public void LoadGameSetting() => GameSetting = Setting.GameSettings.FirstOrDefault(gs => gs.Type == _gameTypeService.Get(SelectedType.Content.ToString()));

    public void SaveSetting()
    {
      if (AutoplaySelected)
        GameSetting.AutoplayNumber = new int?(SelectedAutoplayValue);
      GameSetting gameSetting = Setting.GameSettings.Find(gs => gs.Type == _gameTypeService.Get(SelectedType.Content.ToString()));
      if (gameSetting != null)
        Setting.GameSettings[Setting.GameSettings.IndexOf(gameSetting)] = GameSetting;
      else
        Setting.GameSettings.Add(GameSetting);
      _settingService.Save(Setting);
      GetLatestSetting();
      LoadGameSetting();
      FileInfo fileInfo = new FileInfo("settings.json");
      if (fileInfo.Exists)
        fileInfo.Delete();
      using (StreamWriter streamWriter = new StreamWriter(fileInfo.Open(FileMode.OpenOrCreate)))
        streamWriter.Write(JsonConvert.SerializeObject(Setting));
    }

    public void LoadSelectedSetting()
    {
      GameSetting gameSetting = Setting.GameSettings.FirstOrDefault(gs => gs.Type == _gameTypeService.Get(SelectedType.Content.ToString()));
      if (gameSetting == null)
        gameSetting = new GameSetting()
        {
          Type = _gameTypeService.Get(SelectedType.Content.ToString())
        };
      GameSetting = gameSetting;
    }
  }
}

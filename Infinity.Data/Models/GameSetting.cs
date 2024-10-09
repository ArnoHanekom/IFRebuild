// Decompiled with JetBrains decompiler
// Type: Infinity.Data.Models.GameSetting
// Assembly: Infinity.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A2C88E43-EACF-400C-AB56-5967E8E08F14
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Data.dll

using Infinity.Data.Constants;
using System;
using System.ComponentModel;
using System.Globalization;


#nullable enable
namespace Infinity.Data.Models
{
  public class GameSetting : INotifyPropertyChanged
  {
    private GameType _gameType { get; set; }

    public GameType Type
    {
      get => _gameType;
      set
      {
        if (_gameType != value)
          _gameType = value;
        OnPropertyChanged(nameof (Type));
      }
    }

    private int? _playTables { get; set; }

    public int? PlayTables
    {
      get => _playTables;
      set
      {
        int? playTables = _playTables;
        int? nullable = value;
        if (!(playTables.GetValueOrDefault() == nullable.GetValueOrDefault() & playTables.HasValue == nullable.HasValue))
          _playTables = value;
        OnPropertyChanged(nameof (PlayTables));
        CalculatedSpinTotal = CalculateSpins;
      }
    }

    private int? _randomNumbers { get; set; }

    public int? RandomNumbers
    {
      get => _randomNumbers;
      set
      {
        int? randomNumbers = _randomNumbers;
        int? nullable = value;
        if (!(randomNumbers.GetValueOrDefault() == nullable.GetValueOrDefault() & randomNumbers.HasValue == nullable.HasValue))
          _randomNumbers = value;
        OnPropertyChanged(nameof (RandomNumbers));
        CalculatedSpinTotal = CalculateSpins;
      }
    }

    private int? _autoplayNumber { get; set; }

    public int? AutoplayNumber
    {
      get => _autoplayNumber;
      set
      {
        int? autoplayNumber = _autoplayNumber;
        int? nullable = value;
        if (!(autoplayNumber.GetValueOrDefault() == nullable.GetValueOrDefault() & autoplayNumber.HasValue == nullable.HasValue))
          _autoplayNumber = value;
        OnPropertyChanged(nameof (AutoplayNumber));
        CalculatedSpinTotal = CalculateSpins;
      }
    }

    private int? _rowLimit { get; set; }

    public int? RowLimit
    {
      get => _rowLimit;
      set
      {
        int? rowLimit = _rowLimit;
        int? nullable = value;
        if (!(rowLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & rowLimit.HasValue == nullable.HasValue))
          _rowLimit = value;
        OnPropertyChanged(nameof (RowLimit));
      }
    }

    private int? _countLimit { get; set; }

    public int? CountLimit
    {
      get => _countLimit;
      set
      {
        int? countLimit = _countLimit;
        int? nullable = value;
        if (!(countLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & countLimit.HasValue == nullable.HasValue))
          _countLimit = value;
        OnPropertyChanged(nameof (CountLimit));
      }
    }

    private int? _gsLimit { get; set; }

    public int? GSLimit
    {
      get => _gsLimit;
      set
      {
        int? gsLimit = _gsLimit;
        int? nullable = value;
        if (!(gsLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & gsLimit.HasValue == nullable.HasValue))
          _gsLimit = value;
        OnPropertyChanged(nameof (GSLimit));
      }
    }

    private int? _r1wLimit { get; set; }

    public int? R1WLimit
    {
      get => _r1wLimit;
      set
      {
        int? r1wLimit = _r1wLimit;
        int? nullable = value;
        if (!(r1wLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & r1wLimit.HasValue == nullable.HasValue))
          _r1wLimit = value;
        OnPropertyChanged(nameof (R1WLimit));
      }
    }

    private int? _twLimit { get; set; }

    public int? TWLimit
    {
      get => _twLimit;
      set
      {
        int? twLimit = _twLimit;
        int? nullable = value;
        if (!(twLimit.GetValueOrDefault() == nullable.GetValueOrDefault() & twLimit.HasValue == nullable.HasValue))
          _twLimit = value;
        OnPropertyChanged(nameof (TWLimit));
      }
    }

    private int _calculatedSpinTotal { get; set; }

    private NumberFormatInfo thousandSeparator { get; set; } = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();

    public int CalculatedSpinTotal
    {
      get => _calculatedSpinTotal;
      set
      {
        if (_calculatedSpinTotal != value)
          _calculatedSpinTotal = value;
        thousandSeparator.NumberGroupSeparator = " ";
        OnPropertyChanged(nameof (CalculatedSpinTotal));
        CalculatedSpinTotalStr = CalculatedSpinTotal.ToString("#,0", thousandSeparator);
      }
    }

    public int CalculateSpins
    {
      get
      {
        int? nullable;
        int num1;
        if (!PlayTables.HasValue)
        {
          num1 = 0;
        }
        else
        {
          nullable = PlayTables;
          num1 = nullable.Value;
        }
        nullable = RandomNumbers;
        int num2;
        if (!nullable.HasValue)
        {
          num2 = 0;
        }
        else
        {
          nullable = RandomNumbers;
          num2 = nullable.Value;
        }
        int num3 = num2;
        nullable = AutoplayNumber;
        int num4;
        if (!nullable.HasValue)
        {
          num4 = 1;
        }
        else
        {
          nullable = AutoplayNumber;
          num4 = nullable.Value;
        }
        int num5 = num4;
        int num6 = num3;
        return num1 * num6 * num5;
      }
    }

    private string _calculatedSpinTotalStr { get; set; } = "0";

    public string CalculatedSpinTotalStr
    {
      get => _calculatedSpinTotalStr;
      set
      {
        if (_calculatedSpinTotalStr != value)
          _calculatedSpinTotalStr = value;
        OnPropertyChanged(nameof (CalculatedSpinTotalStr));
      }
    }

    private int _currentSpin { get; set; }

    public int CurrentSpin
    {
      get => _currentSpin;
      set
      {
        if (_currentSpin != value)
          _currentSpin = value;
        OnPropertyChanged(nameof (CurrentSpin));
      }
    }

        private int _selectedLimitFocus { get; set; }
        public int SelectedLimitFocus
        {
            get => _selectedLimitFocus;
            set
            {
                _selectedLimitFocus = value;
                OnPropertyChanged(nameof(SelectedLimitFocus));
            }
        }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = PropertyChanged!;
      if (propertyChanged == null)
        return;
      propertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

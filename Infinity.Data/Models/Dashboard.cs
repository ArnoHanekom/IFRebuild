// Decompiled with JetBrains decompiler
// Type: Infinity.Data.Models.Dashboard
// Assembly: Infinity.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A2C88E43-EACF-400C-AB56-5967E8E08F14
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Data.dll

using Infinity.Data.Constants;
using System;
using System.ComponentModel;


#nullable enable
namespace Infinity.Data.Models
{
    public class Dashboard : INotifyPropertyChanged
    {
        private Table[] _playTables { get; set; } = default!;

        public Table[] PlayTables
        {
            get => _playTables;
            set
            {
                if (_playTables != value)
                    _playTables = value;
                OnPropertyChanged(nameof(PlayTables));
                UpdateCalculatedSpins();
            }
        }

        private int _calculatedSpins { get; set; }

        public int CalculatedSpins
        {
            get => _calculatedSpins;
            set
            {
                if (_calculatedSpins != value)
                    _calculatedSpins = value;
                OnPropertyChanged(nameof(CalculatedSpins));
                UpdateSpinPercentage();
            }
        }

        private int _totalAutoplays { get; set; }

        public int TotalAutoplays
        {
            get => _totalAutoplays;
            set
            {
                if (_totalAutoplays != value)
                    _totalAutoplays = value;
                OnPropertyChanged(nameof(TotalAutoplays));
                UpdateCalculatedSpins();
            }
        }

        private int _totalRandomize { get; set; }

        public int TotalRandomize
        {
            get => _totalRandomize;
            set
            {
                if (_totalRandomize != value)
                    _totalRandomize = value;
                OnPropertyChanged(nameof(TotalRandomize));
                UpdateCalculatedSpins();
            }
        }

        private GameSetting _dashSetting { get; set; } = default!;

        public GameSetting DashSetting
        {
            get => _dashSetting;
            set
            {
                if (_dashSetting != value)
                    _dashSetting = value;
                OnPropertyChanged(nameof(DashSetting));
            }
        }

        private GameType _dashGameType { get; set; }

        public GameType DashGameType
        {
            get => _dashGameType;
            set
            {
                if (_dashGameType != value)
                    _dashGameType = value;
                OnPropertyChanged(nameof(DashGameType));
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
                OnPropertyChanged(nameof(CurrentSpin));
                UpdateSpinPercentage();
            }
        }

        private double _spinPercentage { get; set; }

        public double SpinPercentage
        {
            get => _spinPercentage;
            set
            {
                if (_spinPercentage != value)
                    _spinPercentage = value;
                OnPropertyChanged(nameof(SpinPercentage));
            }
        }

        private void UpdateSpinPercentage() => SpinPercentage = Math.Round(100.0 * CurrentSpin / CalculatedSpins, 2);

        private void UpdateCalculatedSpins() => CalculatedSpins = PlayTables.Length * TotalAutoplays * TotalRandomize;

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

// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.SpinfileGeneratorViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Roulette.Statics;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;


#nullable enable
namespace Infinity.Roulette.ViewModels
{
  public class SpinfileGeneratorViewModel : ViewModelBase
  {
    private int _inumAmount { get; set; }

    private string _numbersAmount { get; set; }

    private string _filename { get; set; }

    public string NumbersAmount
    {
      get => _numbersAmount;
      set
      {
        _numbersAmount = value;
        OnPropertyChanged(nameof (NumbersAmount));
      }
    }

    public string Filename
    {
      get => _filename;
      set
      {
        _filename = value;
        OnPropertyChanged(nameof (Filename));
      }
    }

    public void Generate()
    {
      if (HasValidName)
      {
        generate();
      }
      else
      {
        int num = (int) MessageBox.Show("Spinfile filename cannot be empty", "Alert", MessageBoxButton.OK);
      }
    }

    private void generate()
    {
      _inumAmount = Convertions.StringToInt(NumbersAmount);
      string str = Filename + ".txt";
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Text file (*.txt)|*.txt";
      saveFileDialog.FileName = str;
      saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      bool? nullable = saveFileDialog.ShowDialog();
      bool flag = true;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 1; index <= _inumAmount; ++index)
        {
          if (index == _inumAmount)
            stringBuilder.Append(string.Format("{0}", RandomNumberGenerator.NextRandomNumber()));
          else
            stringBuilder.AppendLine(string.Format("{0}", RandomNumberGenerator.NextRandomNumber()));
        }
        File.WriteAllText(saveFileDialog.FileName, stringBuilder.ToString());
      }
      _generated = true;
      int num = (int) MessageBox.Show("New spinfile \"" + Filename + ".txt\" generated", "Alert", MessageBoxButton.OK);
    }

    private bool HasValidName => Filename != "";

    private bool _generated { get; set; }

    public bool Generated => _generated;
  }
}

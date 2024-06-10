// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Converters.FontFamilyConverter
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


#nullable enable
namespace Infinity.Roulette.Converters
{
  public class FontFamilyConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      FontFamily fontFamily = new FontFamily("Century Gothic");
      if (value != null)
        fontFamily = new FontFamily(value.ToString());
      return fontFamily;
    }

    public object? ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return null;
    }
  }
}

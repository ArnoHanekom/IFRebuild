// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Converters.NullableValueConverter
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using System;
using System.Globalization;
using System.Windows.Data;


#nullable enable
namespace Infinity.Roulette.Converters
{
    public class NullableValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

        public object? ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
        {
            return value != null && string.IsNullOrEmpty(value.ToString()) ? null : value;
        }
    }
}

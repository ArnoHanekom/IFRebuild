// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Statics.Convertions
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll


#nullable enable
namespace Infinity.Roulette.Statics
{
  public static class Convertions
  {
    public static string IntToString(int input) => input <= 0 ? "" : string.Format("{0}", input);

    public static bool IsValidNumber(string? checkString) => checkString != null && !(checkString == "") && int.TryParse(checkString, out int _);

    public static int StringToInt(
    #nullable disable
    string input)
    {
      int result;
      return Convertions.IsValidNumber(input) && int.TryParse(input, out result) ? result : 0;
    }
  }
}

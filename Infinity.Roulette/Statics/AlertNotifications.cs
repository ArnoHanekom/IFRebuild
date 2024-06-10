// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Statics.AlertNotifications
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using System.Media;
using System.Windows;


#nullable enable
namespace Infinity.Roulette.Statics
{
  public static class AlertNotifications
  {
    public static void PlayAlert(string soundFileLocation) => new SoundPlayer()
    {
      SoundLocation = soundFileLocation
    }.Play();

    public static void DisplayAlertMessage(string msg)
    {
      int num = (int) MessageBox.Show(msg, "Alert", MessageBoxButton.OK);
    }
  }
}

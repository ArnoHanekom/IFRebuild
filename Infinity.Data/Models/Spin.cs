// Decompiled with JetBrains decompiler
// Type: Infinity.Data.Models.Spin
// Assembly: Infinity.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A2C88E43-EACF-400C-AB56-5967E8E08F14
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Data.dll

namespace Infinity.Data.Models
{
  public class Spin
  {
    public int TableNumber { get; set; }

    public int Autoplays { get; set; }

    public int Randoms { get; set; }

    public int TotalSpins { get; set; }

    public int CurrentSpin { get; set; }

    public int CurrentAutoplay { get; set; }

    public bool CurrentAutoplayDone { get; set; }

    public bool DoneAutoCalc => CurrentSpin == Randoms;

    public int TotalSpinsDone { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Infinity.Data.Models.SpinWatcher
// Assembly: Infinity.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A2C88E43-EACF-400C-AB56-5967E8E08F14
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Data.dll

using System;

namespace Infinity.Data.Models
{
  public class SpinWatcher
  {
    public int CurrentSpin { get; set; }

    public int TotalSpins { get; set; }

    public double SpinProgress => Math.Round(CurrentSpin / (double) TotalSpins * 100.0, 2);
  }
}

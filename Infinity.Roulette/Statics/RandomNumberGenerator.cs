// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Statics.RandomNumberGenerator
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using System;


#nullable enable
namespace Infinity.Roulette.Statics
{
  public static class RandomNumberGenerator
  {
    private static readonly Random rand = new Random();
    private static readonly object synclock = new object();

    public static int NextRandomNumber()
    {
      lock (RandomNumberGenerator.synclock)
        return RandomNumberGenerator.rand.Next(1, 37);
    }
  }
}

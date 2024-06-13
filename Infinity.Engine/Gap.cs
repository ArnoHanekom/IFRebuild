// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.Gap
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Infinity.Engine
{
  public class Gap
  {
    private int _gapSize;
    private decimal _betSize;
    private int _spinSize;
    private List<KeyValuePair<int, decimal>> _spinBets;

    private int[] _betIncrementSequence { get; set; } = new int[35]
    {
      1,
      1,
      1,
      1,
      2,
      2,
      2,
      3,
      3,
      4,
      5,
      6,
      7,
      8,
      10,
      12,
      15,
      18,
      22,
      25,
      30,
      36,
      44,
      52,
      64,
      76,
      92,
      110,
      132,
      158,
      200,
      230,
      280,
      335,
      400
    };

    public Gap(int gapSize, decimal betSize, int spinSize)
    {
      _gapSize = gapSize;
      _betSize = betSize;
      _spinSize = spinSize;
      InitializeSpinBets();
    }

    public static Gap GetNewGap(int gapSize)
    {
      decimal betSize;
      int spinSize;
      switch (gapSize)
      {
        case 54:
          betSize = 1M;
          spinSize = 35;
          break;
        case 60:
          betSize = 2M;
          spinSize = 31;
          break;
        case 66:
          betSize = 5M;
          spinSize = 26;
          break;
        case 72:
          betSize = 10M;
          spinSize = 22;
          break;
        case 78:
          betSize = 25M;
          spinSize = 17;
          break;
        case 84:
          betSize = 50M;
          spinSize = 14;
          break;
        case 90:
          betSize = 100M;
          spinSize = 10;
          break;
        case 96:
          betSize = 100M;
          spinSize = 10;
          break;
        case 102:
          betSize = 100M;
          spinSize = 10;
          break;
        case 108:
          betSize = 200M;
          spinSize = 7;
          break;
        default:
          betSize = 1M;
          spinSize = 35;
          break;
      }
      return new Gap(gapSize, betSize, spinSize);
    }

    public List<KeyValuePair<int, decimal>> SpinBets => _spinBets;

    public int GapSize
    {
      get => _gapSize;
      set
      {
        _gapSize = value;
        Gap newGap = Gap.GetNewGap(_gapSize);
        _betSize = newGap.BetSize;
        _spinSize = newGap.SpinSize;
        InitializeSpinBets();
      }
    }

    public decimal BetSize => _betSize;

    public int SpinSize => _spinSize;

    private void InitializeSpinBets()
    {
      _spinBets = new List<KeyValuePair<int, decimal>>();
      for (int key = 1; key <= _spinSize; ++key)
        _spinBets.Add(new KeyValuePair<int, decimal>(key, _betIncrementSequence[key - 1] * _betSize));
    }
  }
}

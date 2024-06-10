// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.BettingStrategy
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Infinity.Engine
{
  public class BettingStrategy
  {
    private int _gapSize;
    private Gap _currentGap;
    private short _cursor;

    public BettingStrategy(int gapSize)
    {
      _gapSize = gapSize;
      _currentGap = Gap.GetNewGap(gapSize);
      _cursor = 0;
    }

    public int GapSize
    {
      get => _gapSize;
      set
      {
        _gapSize = value;
        _currentGap.GapSize = _gapSize;
        _cursor = 0;
      }
    }

    public KeyValuePair<int, Decimal> SpinBet => _currentGap.SpinBets[_cursor];

    public Gap CurrentGap => _currentGap;

    public void Spin(bool win) => _cursor = _cursor == _currentGap.SpinSize - 1 || win ? (short) 0 : _cursor++;
  }
}

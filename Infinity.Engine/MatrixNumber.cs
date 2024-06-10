// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.MatrixNumber
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll


#nullable enable
namespace Infinity.Engine
{
  public class MatrixNumber
  {
    private int _number { get; set; }

    private bool _win { get; set; }

    private int _column { get; set; }

    private int _row { get; set; }

    public MatrixNumber(int number, int column, int row)
    {
      _number = number;
      _column = column;
      _row = row;
    }

    public bool Win
    {
      get => _win;
      set => _win = value;
    }

    public int Column => _column;

    public int Row
    {
      get => _row;
      set => _row = value;
    }

    public int Number => _number;

    public override string ToString() => !_win ? string.Format("{0}", _number) : string.Format("{0}w", _number);
  }
}

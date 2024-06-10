// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.BoardColumn
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Engine
{
  public class BoardColumn : IComparable<BoardColumn>
  {
    private int _code { get; set; }

    private List<BoardNumber> _numbers { get; set; }

    private int _columnWins { get; set; }

    public BoardColumn(int code, List<BoardNumber> numbers)
    {
      _code = code;
      _numbers = numbers;
    }

    public int Code
    {
      get => _code;
      set => _code = value;
    }

    public int ColumnWins => _columnWins;

    public List<BoardNumber> Numbers
    {
      get => _numbers;
      set => _numbers = value;
    }

    public BoardNumber? FindNumber(int number) => _numbers.FirstOrDefault(bn => bn.Number == number);

    public BoardNumber? FindNumberWithCode(int boardCode) => _numbers.FirstOrDefault(bn => bn.HasBoardCode(boardCode));

    public BoardNumber? FindNumberWithCodes(List<int> boardCodes) => _numbers.FirstOrDefault(bn => boardCodes.Count(bc => !bn.HasBoardCode(bc)) == 0);

    public List<BoardNumber> FindNumbersWithCodes(List<int> boardCodes) => _numbers.Where(bn => boardCodes.Count(bc => !bn.HasBoardCode(bc)) == 0).ToList();

    public int CompareTo(BoardColumn other) => _code.CompareTo(other.Code);

    public void AddWin() => ++_columnWins;

    public void ResetWins() => _columnWins = 0;
  }
}

// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.BoardNumber
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Engine
{
  public class BoardNumber
  {
    private List<int> _codes { get; set; }

    private int _number { get; set; }

    private int _boardCode { get; set; }

    public BoardNumber(int number, int boardCode)
    {
      _number = number;
      _boardCode = boardCode;
      _codes = new List<int>(6) { 1, 2, 3, 4, 5, 6 };
    }

    public List<int> Codes
    {
      get => _codes;
      set => _codes = value;
    }

    public int Number
    {
      get => _number;
      set => _number = value;
    }

    public int BoardCode
    {
      get => _boardCode;
      set => _boardCode = value;
    }

    public bool Immovable => _codes.Count() == 1;

    public void RemoveCode(int code) => _codes.Remove(code);

    public override string ToString() => string.Format("{0}:{1} {{{2}}}", _number, _boardCode, string.Join(".", _codes));

    public bool HasBoardCode(int boardCode) => _codes.Any(c => c == boardCode);
  }
}

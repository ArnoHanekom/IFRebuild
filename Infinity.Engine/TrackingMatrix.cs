// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.TrackingMatrix
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Engine
{
  public class TrackingMatrix
  {
    private List<List<MatrixNumber>> _columns { get; set; }

    private int _numberOfColumns { get; set; }

    private int _matrixColumn { get; set; }

    private int _maxVerticalGapColumn { get; set; }

    private List<Counter> _columnGapCounters { get; set; }

    public TrackingMatrix(int numberOfColumns)
    {
      _columnGapCounters = new List<Counter>();
      _numberOfColumns = numberOfColumns;
      _columns = new List<List<MatrixNumber>>(numberOfColumns);
      for (int numberValue = 1; numberValue <= _numberOfColumns; ++numberValue)
      {
        _columnGapCounters.Add(new Counter(numberValue, 0));
        _columns.Add(new List<MatrixNumber>());
      }
      _maxVerticalGapColumn = 0;
    }

    public List<List<MatrixNumber>> Columns
    {
      get => _columns;
      set => _columns = value;
    }

    public int MaxVerticalGapSize
    {
      get
      {
        Counter counter = _columnGapCounters.OrderByDescending(cgc => cgc.Count).First();
        _maxVerticalGapColumn = counter.NumberValue;
        return counter.Count;
      }
    }

    public int MaxVerticalGapColumn => _maxVerticalGapColumn;

    public int CurrentColumn => _matrixColumn;

    public int MaxColumnLength => _columns.OrderByDescending(c => c.Count()).First().Count();

    public int NumberOfColumns => _numberOfColumns;

    public void OnSpin(object sender, MatrixSpinEventArgs e) => EnterSpin(e.Number.Number, e.Number.Win);

    public void InitializeCounters()
    {
      for (int numberValue = 1; numberValue <= _numberOfColumns; ++numberValue)
        _columnGapCounters.Add(new Counter(numberValue, 0));
    }

    public void EnterSpin(int number, bool win)
    {
      List<MatrixNumber> column = _columns[_matrixColumn];
      column.Add(new MatrixNumber(number, _matrixColumn + 1, column.Count() + 1)
      {
        Win = win
      });
      Counter columnGapCounter = _columnGapCounters[_matrixColumn];
      ++columnGapCounter.Count;
      if (win)
      {
        columnGapCounter.Count = 0;
        DeleteColumn(_matrixColumn);
      }
      _matrixColumn = _matrixColumn == _numberOfColumns - 1 ? 0 : _matrixColumn++;
    }

    public void DeleteColumn(int columnIndex) => _columns[columnIndex] = new List<MatrixNumber>();
  }
}

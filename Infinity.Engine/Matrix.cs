// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.Matrix
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Engine
{
  public class Matrix
  {
    private List<MatrixRow> _rows { get; set; } = new List<MatrixRow>();

    private MatrixRow _currentRow { get; set; } = new MatrixRow();

    private int _maxGapSize { get; set; }

    private short _numberOfColumns { get; set; } = 6;

    public Matrix()
    {
    }

    public Matrix(short numberOfColumns)
      : this()
    {
      _numberOfColumns = numberOfColumns;
    }

    private Matrix(
      short numberOfColumns,
      List<MatrixRow> rows,
      MatrixRow currentRow,
      int maxGapSize)
    {
      _numberOfColumns = numberOfColumns;
      _rows = rows;
      _currentRow = currentRow;
      _maxGapSize = maxGapSize;
    }

    public List<MatrixRow> Rows
    {
      get => _rows;
      set => _rows = value;
    }

    public MatrixNumber EnterSpin(int number, int boardColumn)
    {
      if (_currentRow.Numbers.Count == 0)
        _rows.Add(_currentRow);
      int column = _currentRow.Numbers.Count() + 1;
      bool flag = boardColumn == column;
      MatrixNumber matrixNumber = new MatrixNumber(number, column, _rows.Count());
      matrixNumber.Win = flag;
      _currentRow.Numbers.Add(matrixNumber);
      if (_currentRow.Numbers.Count == _numberOfColumns)
        _currentRow = new MatrixRow();
      return matrixNumber;
    }

    public List<MatrixNumber> GetAllOccurences(int number)
    {
      List<MatrixNumber> allOccurences = new List<MatrixNumber>();
      for (int index = 0; index < _rows.Count(); ++index)
        allOccurences.AddRange(_rows[index].Numbers.FindAll(x => x.Number == number));
      return allOccurences;
    }

    public MatrixNumber? FindFirstOccurence(int number, int boardCode)
    {
      foreach (MatrixNumber allOccurence in GetAllOccurences(number))
      {
        if (boardCode != allOccurence.Column && !IsInColumn(allOccurence.Number, allOccurence.Column, allOccurence.Row + 1, _rows))
          return allOccurence;
      }
      return null;
    }

    public bool IsInColumn(int number, int column, int startRow) => IsInColumn(number, column, startRow, _rows);

    public bool IsInColumn(int number, int column, int startRow, List<MatrixRow> rows)
    {
      for (int index = startRow - 1; index < rows.Count; ++index)
      {
        if (rows[index].Numbers.Count >= column && rows[index].Numbers[column - 1].Number == number)
          return true;
      }
      return false;
    }

    public List<MatrixRow> DeleteRows(MatrixNumber numberToDelete)
    {
      List<MatrixRow> matrixRowList = new List<MatrixRow>();
      for (int index = 0; index < numberToDelete.Row; ++index)
      {
        MatrixRow row = _rows[index];
        matrixRowList.Add(row);
      }
      _rows.RemoveRange(0, numberToDelete.Row);
      RenumberNumberRows();
      return matrixRowList;
    }

    public int MaxGapSize
    {
      get
      {
        short num = 0;
        _maxGapSize = 0;
        foreach (MatrixRow row in _rows)
        {
          foreach (MatrixNumber number in row.Numbers)
          {
            if (number.Win)
            {
              if (num > _maxGapSize)
                _maxGapSize = num;
              num = 0;
            }
            else
              ++num;
          }
        }
        return num <= _maxGapSize ? _maxGapSize : num;
      }
    }

    public int GapSize
    {
      get
      {
        short gapSize = 0;
        for (int index1 = _rows.Count - 1; index1 >= 0; --index1)
        {
          for (int index2 = _rows[index1].Numbers.Count - 1; index2 >= 0; --index2)
          {
            if (_rows[index1].Numbers[index2].Win)
              return gapSize;
            ++gapSize;
          }
        }
        return gapSize;
      }
    }

    public int CurrentColumn => _currentRow.Numbers.Count();

    private void RenumberNumberRows()
    {
      for (int index1 = 0; index1 < _rows.Count; ++index1)
      {
        List<MatrixNumber> matrixNumberList = new List<MatrixNumber>(6);
        for (int index2 = 0; index2 < _rows[index1].Numbers.Count; ++index2)
        {
          MatrixNumber number = _rows[index1].Numbers[index2];
          matrixNumberList.Add(new MatrixNumber(number.Number, number.Column, index1 + 1)
          {
            Win = number.Win
          });
        }
        _rows[index1].Numbers = matrixNumberList;
      }
    }
  }
}

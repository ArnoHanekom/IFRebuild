// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.Matrix
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private int lastAddedRowColumn()
        {
            for (int i = _currentRow.ColumnNumbers.Length; i > 0; i--)
            {
                MatrixNumber? colNum = _currentRow.ColumnNumbers[i - 1];
                if (colNum is not null)
                    return i;
            }
            return 0;
        }

        private int _currentRowColumnIndex { get; set; }

        public MatrixNumber EnterSpin(int number, int boardColumn, int selectedBoard, int[] arrangement)
        {
            if (_currentRow.Numbers.Count == 0)
                _rows.Add(_currentRow);

            //var lastAddedColumn = lastAddedRowColumn();
            //int nextColumn = lastAddedColumn;
            //if (nextColumn > 0 && nextColumn == selectedBoard + 1)
            //{
            //    nextColumn = selectedBoard + 1 == 6 ? 0 : selectedBoard + 1;
            //}

            //int column = nextColumn == 0 ? 1 : nextColumn + 1; //_currentRow.Numbers.Count() + 1;
            int column = arrangement[selectedBoard];
            _currentRowColumnIndex = arrangement[selectedBoard] - 1;
            bool flag = boardColumn == column;
            MatrixNumber matrixNumber = new(number, column, _rows.Count)
            {
                Win = flag
            };
            _currentRow.ColumnNumbers[arrangement[selectedBoard] - 1] = matrixNumber;
            _currentRow.Numbers.Add(matrixNumber);

            Debug.WriteLine($"Current Rows: {_rows.Count}");
            Debug.WriteLine($"Board Arrangement: {string.Join("\t", arrangement)}");
            Debug.WriteLine($"Selected Row Board: {arrangement[selectedBoard]}");
            Debug.WriteLine($"Current Row Col numbers: \tCol 1: {_currentRow.ColumnNumbers[0]}\tCol 2: {_currentRow.ColumnNumbers[1]}\tCol 3: {_currentRow.ColumnNumbers[2]}\tCol 4: {_currentRow.ColumnNumbers[3]}\tCol 5: {_currentRow.ColumnNumbers[4]}\tCol 6: {_currentRow.ColumnNumbers[5]} ");
            if (_currentRow.Numbers.Count == _numberOfColumns || column == 6)
                _currentRow = new MatrixRow();

            return matrixNumber;
        }

        public List<MatrixNumber> GetAllOccurences(int number)
        {
            List<MatrixNumber> allOccurences = [];
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

        public static bool IsInColumn(int number, int column, int startRow, List<MatrixRow> rows)
        {
            for (int index = startRow - 1; index < rows.Count; ++index)
            {
                if (rows[index].Numbers.Count >= column && rows[index].ColumnNumbers[column - 1].Number == number)
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
            RenumberNumberRows_New();
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
                    for (int index2 = 5; index2 >= 0; --index2)
                    {
                        MatrixNumber? rowNumber = _rows[index1].ColumnNumbers[index2];
                        if (rowNumber is not null)
                        {
                            if (rowNumber.Win)
                                return gapSize;
                            ++gapSize;
                        }
                    }

                    //for (int index2 = _rows[index1].Numbers.Count - 1; index2 >= 0; --index2)
                    //{
                    //    if (_rows[index1].Numbers[index2].Win)
                    //        return gapSize;
                    //    ++gapSize;
                    //}
                }
                return gapSize;
            }
        }

        public int CurrentColumn => _currentRow.Numbers.Count > 0 ? _currentRowColumnIndex + 1 : 0;

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

        private void RenumberNumberRows_New()
        {
            for (int index1 = 0; index1 < _rows.Count; ++index1)
            {
                List<MatrixNumber> matrixNumberList = new(6);
                for (int index2 = 0; index2 < 6; ++index2)
                {
                    MatrixNumber? number = _rows[index1].ColumnNumbers[index2];
                    if (number is not null)
                    {
                        MatrixNumber newNumber = new(number.Number, number.Column, index1 + 1)
                        {
                            Win = number.Win
                        };
                        matrixNumberList.Add(newNumber);
                        _rows[index1].ColumnNumbers[index2] = newNumber;
                    }                    
                }
                _rows[index1].Numbers = matrixNumberList;
            }
        }
    }
}

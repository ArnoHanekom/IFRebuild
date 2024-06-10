// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.Extensions
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Engine
{
    public static class Extensions
    {
        public static void AddPermut(this List<List<int>> CurrentPermutList, int[] permutNums) => CurrentPermutList?.Add(permutNums.Where(p => p > 0).ToList());

        public static void CreateBoardNumbers(
          this List<BoardNumber> numberList,
          int boardNumber,
          int boardCode)
        {
            for (int index = 1; index <= 6; ++index)
            {
                numberList.Add(new BoardNumber(boardNumber, boardCode));
                ++boardNumber;
            }
        }

        public static int GetLastBoardNumber(this List<BoardNumber> numberList) => numberList.OrderByDescending(bn => bn.Number).First().Number;

        public static void CreateAddBoards(
          this List<BoardColumn> columnList,
          List<BoardNumber> numbersList,
          int boardCode)
        {
            columnList.Add(new BoardColumn(boardCode, numbersList));
        }

        public static List<BoardNumber> SearchBoardForValidSwapNumbers(
          this List<BoardColumn> boardColumns,
          int swapCode)
        {
            List<BoardNumber> numbers = new List<BoardNumber>();
            boardColumns.Where(c => c.Code != swapCode).ToList().ForEach(bc =>
            {
                BoardColumn boardColumn = bc;
                numbers.AddRange(boardColumn.FindNumbersWithCodes(new List<int>()
              {
          swapCode
              }));
            });
            return numbers;
        }

        public static List<BoardNumber> SearchBoardForValidSwapNumbers(
          this List<BoardColumn> boardColumns,
          int swapCode,
          int finalSwapCode)
        {
            List<BoardNumber> numbers = new List<BoardNumber>();
            boardColumns.Where(c => c.Code != swapCode).ToList().ForEach(bc =>
            {
                BoardColumn boardColumn = bc;
                numbers.AddRange(boardColumn.FindNumbersWithCodes(new List<int>()
              {
          swapCode,
          finalSwapCode
              }));
            });
            return numbers;
        }

        public static BoardLayout GetBoardLayout(this RouletteGame game, int layoutCode) => game.BoardLayouts[layoutCode - 1];

        public static int HighestRowWin(this BoardLayout boardLayout) => boardLayout.CodeWins.OrderByDescending(cw => cw.Value).First().Key;

        public static int HighestColumnWin(this List<BoardColumn> columns)
        {
            BoardColumn boardColumn = columns.OrderByDescending(c => c.ColumnWins).First();
            return boardColumn == null ? 0 : boardColumn.ColumnWins;
        }

        public static int ColumnWinZeroCount(this List<BoardColumn> columns) => columns.Count(c => c.ColumnWins == 0);

        public static int ColumnWinOneCount(this List<BoardColumn> columns) => columns.Count(c => c.ColumnWins == 1);

        public static void ResetGameColumnsWins(this RouletteGame game) => game.BoardLayouts[0].Columns.ResetColumnsWins();

        public static void ResetColumnsWins(this List<BoardColumn> columns)
        {
            for (int index = 0; index < columns.Count(); ++index)
                columns[index].ResetWins();
        }

        public static void ResetGameLayoutCodeWins(this RouletteGame game) => game.BoardLayouts[0].ResetLayoutCodeWins();

        public static void ResetLayoutCodeWins(this BoardLayout boardLayout) => boardLayout.ResetCodeWins();

        public static BoardNumber ApplyNonCountImmovability(
          this BoardNumber boardNumber,
          MatrixNumber? matrixNumber)
        {
            if (matrixNumber != null)
                boardNumber.Codes.Add(matrixNumber.Column);
            return boardNumber;
        }

        public static int GetCounts(this RouletteGame game)
        {
            lock (game)
            {
                lock (game.BoardLayouts)
                {
                    var numbers = game.BoardLayouts[0].Columns.SelectMany(c => c.Numbers).ToList();
                    return numbers.Count(n => n.Codes.Count() <= 1);
                }
            }
            
            //int counts = 0;
            //lock (game.BoardLayouts[0].Columns)
            //{
            //    foreach (BoardColumn column in game.BoardLayouts[0].Columns)
            //    {
            //        lock (column.Numbers)
            //        {
            //            foreach (BoardNumber number in column.Numbers)
            //            {
            //                if (number.Codes.Count() == 1)
            //                    ++counts;
            //            }
            //        }
            //    }
            //}
            //return counts;
        }

        public static int GetRows(this RouletteGame game) => game.BoardLayouts[0].Matrix.Rows.Count();

        public static int GetGS(this RouletteGame game) => game.BoardLayouts[0].Matrix.GapSize;

        public static int GetMaxGS(this RouletteGame game) => game.BoardLayouts[0].Matrix.MaxGapSize;

        public static SpinPhase GetActiveSpinPhase(this BoardLayout boardLayout)
        {
            return boardLayout.Phases
                .First(p => p.Started && !p.Ended);
        }

        public static void UpdateTableGuid(this RouletteGame game, Guid tableGuid)
        {
            game.TableUniqueId = tableGuid;
        }

        public static void UpdateCurrentSpin(this RouletteGame game, int spinno)
        {
            game.UpdateCurrentSpinNo(spinno);
        }

        public static CountNumber ToCountNumber(this BoardNumber number)
        {
            return new(number.Number, number.BoardCode);
        }
    }
}

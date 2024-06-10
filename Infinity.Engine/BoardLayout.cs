// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.BoardLayout
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using Infinity.Engine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


#nullable enable
namespace Infinity.Engine
{
    public class BoardLayout
    {
        private int _layoutCode;
        private int _bettingStategyGap;
        private int _maxVerticalGapMatrix;
        private int _layoutWins;
        private Func<int, int, int> CalculatedRow = (boardCode, matrixColumn) =>
        {
            if (boardCode < matrixColumn)
                return 6 - (matrixColumn - boardCode);
            return boardCode > matrixColumn ? boardCode - matrixColumn : 0;
        };

        private List<BoardColumn> _columns { get; set; } = new List<BoardColumn>(6);

        private Matrix _matrix { get; set; } = new Matrix();

        private List<TrackingMatrix> _matrices { get; set; } = new List<TrackingMatrix>(50);

        private BettingStrategy _bettingStrategy { get; set; }

        private Dictionary<int, int> _codeWins { get; set; }

        public Dictionary<int, int> CodeWins => _codeWins;

        public event MatrixSpinEventHandler MatrixSpin;

        private SpinPhase _phase { get; set; } = new();
        private List<SpinPhase> _phases { get; set; } = new();
        public SpinPhase Phase => _phase;
        public List<SpinPhase> Phases => _phases;

        private int? _countSpinType { get; set; } = null;

        public int? CountSpinType => _countSpinType;

        public void ResetCodeWins() => resetCodeWins();
        private void resetCodeWins()
        {
            _codeWins = new Dictionary<int, int>(6);
            for (int key = 0; key < 6; ++key)
                _codeWins.Add(key, 0);
        }

        public void ResetCountSpinType() => resetCountSpinType();

        private void resetCountSpinType()
        {
            _countSpinType = null;
        }
        public void UpdateCountSpinType(int spinType) => updateCountSpinType(spinType);
        private void updateCountSpinType(int spinType) => _countSpinType = spinType;

        public void EndPhase() => endPhase();
        private void endPhase()
        {
            lock (_phases)
            {
                var prevPhaseId = 0;
                _phases.Where(p => p.Started && !p.Ended)
                    .ToList()
                    .ForEach(p => {
                        p.Ended = true;
                        prevPhaseId = p.Id;
                    });

                var nextPhaseId = prevPhaseId + 1;
                var nextPhase = new SpinPhase(id: nextPhaseId, key: $"Phase {nextPhaseId}", startedBy: CountSpinType) { Started = true, Ended = false };
                _phases.Add(nextPhase);
            }
        }
        private readonly IEngineService _engineService;
        public BoardLayout(IEngineService engineService)
        {
            _engineService = engineService;
            resetCodeWins();
            for (short numberOfColumns = 2; numberOfColumns <= 50; ++numberOfColumns)
            {
                TrackingMatrix trackingMatrix = new TrackingMatrix(numberOfColumns);
                MatrixSpin += new MatrixSpinEventHandler(trackingMatrix.OnSpin);
                _matrices.Add(trackingMatrix);
            }
            _maxVerticalGapMatrix = 0;
            
        }

        private bool isReset { get; set; }
        public bool IsReset => isReset;

        public void SetReset(bool reset)
        {
            isReset = reset;
        }

        public int LayoutWins => _layoutWins;

        public int LayoutCode => _layoutCode;

        public List<BoardColumn> Columns
        {
            get => _columns;
            set => _columns = value;
        }

        public Matrix Matrix => _matrix;

        public List<TrackingMatrix> TrackingMatrices => _matrices;

        public int BettingStrategyGapSize
        {
            get => _bettingStategyGap;
            set => _bettingStategyGap = value;
        }

        public int MaxGapSize
        {
            get
            {
                int maxVerticalGapSize = MaxVerticalGapSize;
                return _matrix.GapSize <= maxVerticalGapSize ? maxVerticalGapSize : Matrix.GapSize;
            }
        }

        public int MaxVerticalGapSize
        {
            get
            {
                int maxVerticalGapSize = 0;
                int num = 0;
                for (int index = 0; index < _matrices.Count; ++index)
                {
                    if (_matrices[index].MaxVerticalGapSize > maxVerticalGapSize)
                    {
                        maxVerticalGapSize = _matrices[index].MaxVerticalGapSize;
                        num = index + 1;
                    }
                }
                _maxVerticalGapMatrix = num;
                return maxVerticalGapSize;
            }
        }

        public int MaxVerticalGapMatrix => _maxVerticalGapMatrix;

        public List<BoardNumber>? BetOnNumbers
        {
            get
            {
                if (!BettingActive)
                    return null;
                return GetBoardColumn(_matrix.CurrentColumn == 6 ? 1 : _matrix.CurrentColumn + 1)?.Numbers;
            }
        }

        public BoardColumn? GetBoardColumn(int boardCode) => _columns.FirstOrDefault(c => c.Code == boardCode);

        public bool BettingActive
        {
            get
            {
                int maxVerticalGapSize = MaxVerticalGapSize;
                int gapSize = _matrix.GapSize;
                if (gapSize >= maxVerticalGapSize && gapSize >= _bettingStategyGap)
                    return true;
                if (maxVerticalGapSize >= gapSize && maxVerticalGapSize > _bettingStategyGap)
                {
                    TrackingMatrix matrix = _matrices[_maxVerticalGapMatrix - 1];
                    if (matrix.CurrentColumn == matrix.MaxVerticalGapColumn)
                        return true;
                }
                return false;
            }
        }

        public BettingStrategy BettingStrategy
        {
            get => _bettingStrategy;
            set => _bettingStrategy = value;
        }

        public void Initialize(int bettingStategyGap, int layoutCode)
        {
            lock (_phases)
            {
                _phases.Clear();
                var nextPhaseId = 1;
                _phase = new(nextPhaseId, $"Phase {nextPhaseId}", -1) { Started = true, Ended = false };
                _phases.Add(_phase);
            }
            List<int> current = Permutations.CurrentList[layoutCode - 1];
            _layoutCode = layoutCode;
            int boardNumber = 1;
            foreach (int boardCode in current)
            {
                List<BoardNumber> boardNumberList = new List<BoardNumber>(6);
                boardNumberList.CreateBoardNumbers(boardNumber, boardCode);
                boardNumber = boardNumberList.GetLastBoardNumber() + 1;
                _columns.CreateAddBoards(boardNumberList, boardCode);
            }
            _columns.Sort();
            _bettingStategyGap = bettingStategyGap;
            _bettingStrategy = new BettingStrategy(bettingStategyGap);
        }

        public BoardNumber ApplyImmovabilty(BoardNumber boardNumber)
        {
            List<MatrixRow> rows = _matrix.DeleteRows(_matrix.FindFirstOccurence(boardNumber.Number, boardNumber.BoardCode));
            for (int i = 1; i <= 6; i++)
            {
                foreach (MatrixRow matrixRow in rows)
                {
                    if (i <= matrixRow.Numbers.Count)
                    {
                        int number1 = matrixRow.Numbers[i - 1].Number;
                        BoardNumber number2 = FindNumber(number1);
                        int num = _matrix.IsInColumn(number1, i, 1) ? 1 : 0;
                        bool flag = _matrix.IsInColumn(number1, i, matrixRow.Numbers[i - 1].Row + 1, rows);
                        if (num == 0 && !flag && !number2.Codes.Any(c => c == i))
                            number2.Codes.Add(i);
                    }
                }
            }
            ResetCodeWins();
            ResetColumnsWins();
            SetReset(true);
            EndPhase();
            ResetCountSpinType();            
            return boardNumber;
        }

        private void ResetColumnsWins() => _columns.ForEach(c => c.ResetWins());

        public BoardNumber? FindNumber(int number) => _columns.FirstOrDefault(c => c.Numbers.Any(n => n.Number == number))?.FindNumber(number);

        public void OnSpin(object sender, SpinEventArgs e) => CaptureSpin(e.Number, e.TableId, e.GameId, e.SpinType, e.PhaseType, e.SpinId);

        private void ProcessGameEndPhase(Guid tableId, Guid gameId, int spinType, int phaseType, Guid spinId)
        {
            Task.Run(async() => 
            {
                await _engineService.IncreaseResetCountAsync(tableId, gameId);
                await _engineService.UpdatePhaseEndAsync(tableId, gameId, spinType);                
                await _engineService.AddGamePhaseAsync(tableId, gameId, phaseType, spinType);
            });
        }

        public void CaptureSpin(int winningNumber, Guid tableId, Guid gameId, int spinType, int phaseType, Guid spinId)
        {
            SetReset(false);
            if (winningNumber != 0)
            {
                BoardNumber boardNumber = FindNumber(winningNumber);
                MatrixNumber matrixNumber = _matrix.EnterSpin(winningNumber, boardNumber.BoardCode);
                int column = matrixNumber.Column;
                if (boardNumber == null)
                    throw new Exception("Computational Error: Number " + winningNumber.ToString() + " not found in board! ");

                _codeWins[CalculatedRow(boardNumber.BoardCode, column)]++;

                boardNumber.RemoveCode(column);
                if (column == boardNumber.BoardCode)
                {
                    matrixNumber.Win = true;
                    GetBoardColumn(boardNumber.BoardCode)!.AddWin();

                    if (boardNumber.Codes.Count == 0)
                        ProcessGameEndPhase(tableId, gameId, spinType, phaseType, spinId);

                    if (boardNumber.Codes.Count == 0)
                        boardNumber = ApplyImmovabilty(boardNumber);


                    if (!Swap(boardNumber))
                    {
                        boardNumber.ApplyNonCountImmovability(_matrix.FindFirstOccurence(boardNumber.Number, boardNumber.BoardCode));
                        if (Swap(boardNumber))
                            ;
                    }
                }
                if (BettingActive)
                    _bettingStrategy.Spin(matrixNumber.Win);
                MatrixSpin(this, new MatrixSpinEventArgs()
                {
                    Number = matrixNumber
                });
            }
            else
            {
                if (!BettingActive)
                    return;
                _bettingStrategy.Spin(false);
            }
        }

        public bool Swap(BoardNumber originalNumber)
        {
            List<BoardNumber> boardNumberList = _columns.SearchBoardForValidSwapNumbers(originalNumber.BoardCode);
            bool flag = false;
            foreach (BoardNumber boardNumber in boardNumberList)
            {
                BoardNumber sn = boardNumber;
                if (originalNumber.Codes.Any(c => c == sn.BoardCode) & sn.Codes.Any(c => c == originalNumber.BoardCode) && originalNumber.Number != sn.Number)
                {
                    SwapNumbers(originalNumber, sn);
                    flag = true;
                    break;
                }
            }
            return flag || DoubleSwap(originalNumber) || TripleSwap(originalNumber);
        }

        private bool TripleSwap(BoardNumber originalNumber)
        {
            bool flag = false;
            lock (originalNumber.Codes)
            {
                foreach (int code1 in originalNumber.Codes)
                {
                    BoardColumn boardColumn1 = GetBoardColumn(code1);
                    lock (boardColumn1.Numbers)
                    {
                        foreach (BoardNumber number1 in boardColumn1.Numbers)
                        {
                            lock (number1.Codes)
                            {
                                foreach (int code2 in number1.Codes)
                                {
                                    BoardColumn boardColumn2 = GetBoardColumn(code2);
                                    lock (boardColumn2.Numbers)
                                    {
                                        foreach (BoardNumber number2 in boardColumn2.Numbers)
                                        {
                                            lock (number2.Codes)
                                            {
                                                foreach (int code3 in number2.Codes)
                                                {
                                                    BoardColumn boardColumn3 = GetBoardColumn(code3);
                                                    lock (boardColumn3.Numbers)
                                                    {
                                                        foreach (BoardNumber number3 in boardColumn3.Numbers)
                                                        {
                                                            lock (number3.Codes)
                                                            {
                                                                if (number3.Number != number1.Number)
                                                                {
                                                                    if (number3.Number != number2.Number)
                                                                    {
                                                                        if (number3.Number != originalNumber.Number)
                                                                        {
                                                                            if (number3.Codes.Contains(number2.BoardCode))
                                                                            {
                                                                                if (number3.Codes.Contains(number1.BoardCode))
                                                                                {
                                                                                    if (number3.Codes.Contains(originalNumber.BoardCode))
                                                                                    {
                                                                                        SwapNumbers(number3, number2);
                                                                                        SwapNumbers(number3, number1);
                                                                                        SwapNumbers(originalNumber, number3);
                                                                                        flag = true;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (flag)
                                                        break;
                                                }
                                            }
                                            if (flag)
                                                break;
                                        }
                                    }
                                    if (flag)
                                        break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                    if (flag)
                        break;
                }
            }
            return flag;
        }

        private bool DoubleSwap(BoardNumber originalNumber)
        {
            bool flag = false;
            lock (_columns)
            {
                lock (originalNumber.Codes)
                {
                    foreach (int code in originalNumber.Codes)
                    {
                        List<BoardNumber> source = _columns.SearchBoardForValidSwapNumbers(code, originalNumber.BoardCode);
                        lock (source)
                        {
                            if (source.Any(ps => ps.Number == originalNumber.Number))
                                source.Remove(originalNumber);
                            foreach (BoardNumber swapNumber in source)
                            {
                                BoardNumber numberWithCode = GetBoardColumn(code).FindNumberWithCode(swapNumber.BoardCode);
                                if (numberWithCode != null)
                                {
                                    SwapNumbers(numberWithCode, swapNumber);
                                    SwapNumbers(originalNumber, swapNumber);
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (flag)
                            break;
                    }
                }
            }
            return flag;
        }

        private void SwapNumbers(BoardNumber originalNumber, BoardNumber swapNumber)
        {
            BoardColumn boardColumn1 = GetBoardColumn(originalNumber.BoardCode);
            BoardColumn boardColumn2 = GetBoardColumn(swapNumber.BoardCode);
            lock (boardColumn1)
            {
                lock (boardColumn2)
                {
                    boardColumn2.Numbers.Remove(swapNumber);
                    boardColumn1.Numbers.Remove(originalNumber);
                    swapNumber.BoardCode = boardColumn1.Code;
                    boardColumn1.Numbers.Add(swapNumber);
                    originalNumber.BoardCode = boardColumn2.Code;
                    boardColumn2.Numbers.Add(originalNumber);
                }
            }
        }
    }
}

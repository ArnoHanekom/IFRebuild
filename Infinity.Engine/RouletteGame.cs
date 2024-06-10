// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.RouletteGame
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using Infinity.Engine.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


#nullable enable
namespace Infinity.Engine
{
    public class RouletteGame
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        private int _bettingGapSize { get; set; }

        private List<BoardLayout> _boardLayouts { get; set; } = new List<BoardLayout>();

        private List<int> _spinHistory { get; set; } = new List<int>();

        private int _spins { get; set; }

        private event SpinEventHandler Spin;
        private readonly IEngineService _engineService;
        public RouletteGame(int bettingGapSize, IEngineService engineService)
        {
            _engineService = engineService;
            _bettingGapSize = bettingGapSize;
            InitializeBoards();
        }

        private void InitializeBoards()
        {
            int[] nums = new int[100];
            for (int index = 1; index <= 6; ++index)
                nums[index] = index;
            Permutations.Permut(1, 6, nums);
            BoardLayout boardLayout = new BoardLayout(_engineService);
            boardLayout.Initialize(_bettingGapSize, 1);
            Spin += new SpinEventHandler(boardLayout.OnSpin);
            _boardLayouts.Add(boardLayout);
            _spinType = 0;
            _spinTypeHistory = new();
        }

        public int Spins
        {
            get => _spins;
            set => _spins = value;
        }

        public List<BoardLayout> BoardLayouts
        {
            get => _boardLayouts;
            set => _boardLayouts = value;
        }

        public List<int> SpinHistory => _spinHistory;

        private int _spinType { get; set; } = 0;
        public int SpinType => _spinType;

        private List<int> _spinTypeHistory { get; set; } = new();
        public List<int> SpinTypeHistory => _spinTypeHistory;

        public void AddSpinTypeHistory(int spinType)
        {
            lock(_spinTypeHistory)
            {
                _spinType = spinType;
                _spinTypeHistory.Add(spinType);
                this.BoardLayouts[0].UpdateCountSpinType(spinType);
            }
        }

        public void CaptureSpin(int winningNumber, int phaseType = 3)
        {
            ++_spins;
            _spinHistory.Add(winningNumber);
            var spinEntity = new SpinEntity() { TableId = TableUniqueId, GameId = GameId, WinningNumber = winningNumber, Type = SpinType };
            Spin(this, new SpinEventArgs()
            {
                Number = winningNumber,
                TableId = TableUniqueId,
                GameId = GameId,
                SpinType = SpinType,
                PhaseType = phaseType,
                SpinId = spinEntity.Id
            });

            lock (this.BoardLayouts[0])
            {
                lock (this.BoardLayouts[0].Columns)
                {
                    var boardNumbers = this.BoardLayouts[0].Columns.SelectMany(c => c.Numbers).ToList();
                    lock (boardNumbers)
                    {
                        if (boardNumbers.Any(bn => bn.Codes.Count() <= 1))
                        {
                            spinEntity.CountNumbers = boardNumbers.Where(bn => bn.Codes.Count() <= 1).Select(bn => bn.ToCountNumber()).ToList();
                            Task.Run(async () => await _engineService.AddSpinEntityAsync(spinEntity));
                        }                        
                    }
                }
            }

            lock (_engineService)
            {
                Task.Run(async () =>
                {
                    var lastOpenPhase = await _engineService.GetLastOpenPhase(TableUniqueId, GameId);
                    if (lastOpenPhase is not null)
                    {
                        await _engineService.AddUpdatePhaseSpin(lastOpenPhase.Id, spinEntity.Id);
                    }
                });
            }
        }

        public string BettingSummary()
        {
            StringBuilder stringBuilder = new StringBuilder();
            short num1 = 0;
            List<KeyValuePair<int, Decimal>> keyValuePairList1 = new List<KeyValuePair<int, Decimal>>();
            Hashtable hashtable = new Hashtable();
            lock (BoardLayouts)
            {
                foreach (BoardLayout boardLayout in BoardLayouts)
                {
                    if (boardLayout.BettingActive && boardLayout.BetOnNumbers != null)
                    {
                        List<KeyValuePair<int, Decimal>> keyValuePairList2 = keyValuePairList1;
                        KeyValuePair<int, Decimal> spinBet = boardLayout.BettingStrategy.SpinBet;
                        int key = spinBet.Key;
                        spinBet = boardLayout.BettingStrategy.SpinBet;
                        Decimal num2 = spinBet.Value;
                        KeyValuePair<int, Decimal> keyValuePair = new KeyValuePair<int, Decimal>(key, num2);
                        keyValuePairList2.Add(keyValuePair);
                        lock (boardLayout.BetOnNumbers)
                        {
                            foreach (BoardNumber betOnNumber in boardLayout.BetOnNumbers)
                            {
                                if (hashtable.ContainsKey(betOnNumber.Number))
                                {
                                    short num3 = (short)(Convert.ToInt16(hashtable[betOnNumber.Number]) + 1);
                                    hashtable[betOnNumber.Number] = num3;
                                }
                                else
                                    hashtable.Add(betOnNumber.Number, 1);
                            }
                        }
                        ++num1;
                    }
                }
            }
            stringBuilder.AppendFormat("{0} Total boards were found\n", num1);
            List<Counter> counterList = new List<Counter>();
            lock (hashtable)
            {
                foreach (DictionaryEntry dictionaryEntry in hashtable)
                    counterList.Add(new Counter(Convert.ToInt32(dictionaryEntry.Key), Convert.ToInt32(dictionaryEntry.Value)));
            }
            lock (counterList)
            {
                counterList.Sort();
                int num4 = 0;
                foreach (Counter counter in counterList)
                {
                    if (counter.Count != num4)
                        stringBuilder.AppendFormat("\n[{0}] occurences of ", counter.Count);
                    stringBuilder.AppendFormat("{0} ", counter.NumberValue);
                    num4 = counter.Count;
                }
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        private Guid tableUniqueId { get; set; }
        public Guid TableUniqueId
        {
            get
            {
                return tableUniqueId;
            }
            set
            {
                tableUniqueId = value;
            }
        }
        private int currentSpinNo {  get; set; } = 0;
        public int CurrentSpinNo => currentSpinNo;
        internal void UpdateCurrentSpinNo(int spinNo)
        {
            currentSpinNo = spinNo;
        }
    }
}

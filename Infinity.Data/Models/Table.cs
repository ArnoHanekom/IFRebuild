using Infinity.Engine;
using Infinity.Engine.Services;

namespace Infinity.Data.Models
{
    public class Table
    {
        private Func<KeyValuePair<int, int>?, bool> _r1wAndHighest = R1Rows => R1Rows.HasValue && R1Rows.Value.Key == 0;
        private Func<KeyValuePair<int, int>?, int, bool> _r1wLimitReached = (R1Rows, r1limit) => R1Rows.HasValue && R1Rows.Value.Value >= r1limit;

        private readonly IEngineService _engineService;        
        //public Table(IEngineService engineService, IOddWinService oddWinService)
        //{
        //    _engineService = engineService;            
        //    Game = new RouletteGame(36, uniqueTableId, _engineService, oddWinService);            
        //}

        public Table(IEngineService engineService)
        {
            _engineService = engineService;
            Game = new RouletteGame(36, engineService);
            Game.UpdateTableGuid(UniqueTableId);
        }
        private KeyValuePair<int, int>? R1Rows
        {
            get
            {
                RouletteGame game = Game;
                return game == null ? new KeyValuePair<int, int>?() : new KeyValuePair<int, int>?(game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault());
            }
        }

        private Guid uniqueTableId = Guid.NewGuid();
        public Guid UniqueTableId => uniqueTableId;
        public int TableId { get; set; }
        public int Autoplay { get; set; }
        public bool ExactMatch { get; set; }
        public bool WinsMatch { get; set; }
        public bool RunSpinfile { get; set; }
        public int FirstRowWin
        {
            get
            {
                RouletteGame game = Game;
                return game == null ? 0 : game.BoardLayouts[0].CodeWins[0];
            }
        }
        public int HighestColumnWin
        {
            get
            {
                RouletteGame game = Game;
                return game == null ? 0 : game.BoardLayouts[0].Columns.HighestColumnWin();
            }
        }
        //public int ColumnWinZeroCount
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        return game == null ? 0 : game.BoardLayouts[0].Columns.ColumnWinZeroCount();
        //    }
        //}
        //public int ColumnWinOneCount
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        return game == null ? 0 : game.BoardLayouts[0].Columns.ColumnWinOneCount();
        //    }
        //}
        //public bool HighlightR1WRow => R1Wlimit.HasValue && _r1wAndHighest(R1Rows) && _r1wLimitReached(R1Rows, R1Wlimit.Value);
        //public bool HighlightTWRow { get; }
        public bool R1WMatch => R1Wlimit.HasValue && FirstRowWin >= R1Wlimit.Value && _r1wAndHighest(R1Rows);
        public bool TWMatch => TWlimit.HasValue && HighestColumnWin >= TWlimit.Value;
        //public bool Spinned { get; set; }
        public int? R1Wlimit { get; set; }
        public int? TWlimit { get; set; }
        public RouletteGame Game { get; private set; }
        public int Rows => Game.GetRows();
        public int Spins => Game.Spins;
        //public int Counts => Game.GetCounts();
        //public int GS => Game.GetGS();
        //public int MaxGS => Game.GetMaxGS();
        //public int CalculatedTotalSpins { get; set; }
        public bool DoneSpinning { get; set; }
        //public int TableFirstRowWins
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        return game == null ? 0 : game.BoardLayouts[0].CodeWins[0];
        //    }
        //}
        //public int TableHighestColumnWin
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        return game == null ? 0 : game.BoardLayouts[0].Columns.HighestColumnWin();
        //    }
        //}
        ////public int TableColumnWinZeroCount
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        return game == null ? 0 : game.BoardLayouts[0].Columns.ColumnWinZeroCount();
        //    }
        //}
        ////public int TableColumnWinOneCount
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        return game == null ? 0 : game.BoardLayouts[0].Columns.ColumnWinOneCount();
        //    }
        //}
        //public bool HighlightR1WinsRow => _highlightR1WinsRow;
        //public bool HighlightTWWinsRow => _highlightTWinsRow;
        //public int? R1WSetLimit { get; set; }
        //public int? TWSetLimit { get; set; }
        //public string CombinedId => string.Format("{0}_{1}", TableId, Autoplay);
        public bool isHighestRowWin
        {
            get
            {
                RouletteGame game = Game;
                return _isHighestAndFirstRow(game != null ? new KeyValuePair<int, int>?(game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault()) : new KeyValuePair<int, int>?());
            }
        }
        private bool _isHighestAndFirstRow(KeyValuePair<int, int>? highestR1Wins) => highestR1Wins.HasValue && highestR1Wins.Value.Key == 0;
        //private bool _isR1WinsEqualHigherThanLimit(KeyValuePair<int, int>? highestR1Wins) => _isR1WLimitSet && highestR1Wins.HasValue && highestR1Wins.Value.Value >= R1WSetLimit!.Value;
        //private bool _isR1WLimitSet => R1WSetLimit.HasValue;
        //private bool _isTWLimitSet => TWSetLimit.HasValue;
        //private bool _highlightR1WinsRow
        //{
        //    get
        //    {
        //        RouletteGame game = Game;
        //        KeyValuePair<int, int>? highestR1Wins = game != null ? new KeyValuePair<int, int>?(game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault()) : new KeyValuePair<int, int>?();
        //        return _isR1WLimitSet && _isHighestAndFirstRow(highestR1Wins) && _isR1WinsEqualHigherThanLimit(highestR1Wins);
        //    }
        //}
        //private bool _highlightTWinsRow => _isTWLimitSet && TableHighestColumnWin >= TWSetLimit!.Value;
        public void SetGame(RouletteGame game)
        {
            Game = game;
            Game.UpdateTableGuid(uniqueTableId);
        }
        public void SetTableId(Guid tableId)
        {
            uniqueTableId = tableId;
        }
        public bool TableR1Match(int? limit) => limit.HasValue && FirstRowWin >= limit.Value && _r1wAndHighest(R1Rows);
        //public int GameBoardResets => _engineService.GetGameResetCount(UniqueTableId, Game.GameId);

    }
}

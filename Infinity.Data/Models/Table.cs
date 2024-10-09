using Infinity.Engine;
using Infinity.Engine.Services;

namespace Infinity.Data.Models
{
    
    public class Table
    {
        private Func<KeyValuePair<int, int>?, bool> _r1wAndHighest = R1Rows => R1Rows.HasValue && R1Rows.Value.Key == 0;
        private Func<KeyValuePair<int, int>?, int, bool> _r1wLimitReached = (R1Rows, r1limit) => R1Rows.HasValue && R1Rows.Value.Value >= r1limit;

        private readonly IEngineService _engineService; 
        public Table(IEngineService engineService)
        {
            _engineService = engineService;
            Game = new RouletteGame(36, engineService);
            Game.UpdateTableGuid(UniqueTableId);
        }
        private KeyValuePair<int, int>? R1Rows => Game == null ? new KeyValuePair<int, int>?() : new KeyValuePair<int, int>?(Game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault());

        private Guid uniqueTableId = Guid.NewGuid();
        public Guid UniqueTableId => uniqueTableId;
        public int TableId { get; set; }
        public int Autoplay { get; set; }
        public bool ExactMatch { get; set; }
        public bool WinsMatch { get; set; }
        public bool RunSpinfile { get; set; }
        public int FirstRowWin => Game?.BoardLayouts[0].CodeWins[0] ?? 0;
        public int HighestColumnWin => Game?.BoardLayouts[0].Columns.HighestColumnWin() ?? 0;
        public int ColumnWinZeroCount => Game?.BoardLayouts[0].Columns.ColumnWinZeroCount() ?? 0;
        public int ColumnWinOneCount => Game?.BoardLayouts[0].Columns.ColumnWinOneCount() ?? 0;
        public bool R1WMatch => R1Wlimit.HasValue && FirstRowWin >= R1Wlimit.Value && _r1wAndHighest(R1Rows);
        public bool TWMatch => TWlimit.HasValue && HighestColumnWin >= TWlimit.Value;
        public int? R1Wlimit { get; set; }
        public int? TWlimit { get; set; }
        public RouletteGame Game { get; private set; }
        public int Rows => Game.GetRows();
        public int Counts => Game.GetCounts();
        public int GS => Game.GetGS();
        public int MaxGS => Game.GetMaxGS();
        public int Spins => Game.Spins;
        public bool DoneSpinning { get; set; }
        public bool isHighestRowWin => _isHighestAndFirstRow(Game != null ? new KeyValuePair<int, int>?(Game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault()) : new KeyValuePair<int, int>?());
        private bool _isHighestAndFirstRow(KeyValuePair<int, int>? highestR1Wins) => highestR1Wins.HasValue && highestR1Wins.Value.Key == 0;
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

        public int Matched => ExactMatch ? 1 : WinsMatch ? 1 : R1WMatch ? 1 : TWMatch ? 1 : 0;
        public int Order => ExactMatch ? 0 : WinsMatch ? 1 : R1WMatch ? 2 : TWMatch ? 3 : 99;
        public int ColumnWithHighestRowWin => Game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault().Key + 1;
        public int ColumnWithHighestRowWinValue => Game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault().Value;

        public bool IsTWSearch { get; set; } = false;  
    }
}

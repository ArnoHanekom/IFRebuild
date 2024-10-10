using Infinity.Engine;
using Infinity.Engine.Services;

namespace Infinity.Data.Models
{

    public class Table
    {
        private Func<KeyValuePair<int, int>?, bool> _r1wAndHighest = R1Rows => R1Rows.HasValue && R1Rows.Value.Key == 0;
        private Func<KeyValuePair<int, int>?, int, bool> _r1wLimitReached = (R1Rows, r1limit) => R1Rows.HasValue && R1Rows.Value.Value >= r1limit;
        private bool _shuffleBoards { get; set; }

        private readonly IEngineService _engineService;
        public Table(IEngineService engineService, bool shuffleBoards = false)
        {
            _engineService = engineService;
            _shuffleBoards = shuffleBoards;
            Game = new RouletteGame(36, engineService);

            if (_shuffleBoards) Game = Game.Shuffle();

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

    public static class TableGameExtensions
    {
        private static Random _rand { get; set; } = new Random();
        public static RouletteGame Shuffle(this RouletteGame game)
        {
            lock (game)
            {
                var layout = game.BoardLayouts[0];
                var shuffled = _shuffledList(_listToShuffle);
                List<List<BoardNumber>> colNumbers = [];

                for (int i = 0; i < 6; i++)
                {
                    var numbers = shuffled.Skip(i * 6).Take(6).ToList();
                    var boardNumbers = numbers.Select(layout.FindNumber).ToList();
                    foreach (var bn in boardNumbers)
                    {
                        bn!.BoardCode = i + 1;
                    }
                    colNumbers.Add(boardNumbers!);
                }
                for (int j = 0; j < 6; j++)
                {
                    layout.Columns[j].Numbers = colNumbers[j];
                }
                return game;
            }            
        }

        private static List<int> _shuffledList(List<int> listToShuffle)
        {
            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
            return listToShuffle;
        }

        private static List<int> _listToShuffle => Enumerable.Range(1, 36).Select(x => x).ToList();
    }
}

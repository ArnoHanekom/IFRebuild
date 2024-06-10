using System.Text;
using System.Diagnostics;
using System.Collections;

namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class RouletteGame
{
    int _bettingGapSize;
    List<BoardLayout> _boardLayouts;
    List<int> _spinHistory;
    int _spins;

    public event SpinEventHandler Spin;

    static RouletteGame _instance;

    public static RouletteGame CreateGame(int bettingSize)
    {
        RouletteGame g = new RouletteGame(bettingSize);
        _instance = g;
        return g;
    }

    private RouletteGame(int bettingGapSize)
    {
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED,
            "Starting Up Roulette Matrix " + DateTime.Now.ToString());

        //initialize boards
        _bettingGapSize = bettingGapSize;
        InitializeBoards();

        //initialize the history list
        _spinHistory = new List<int>();

        //set defaults
        _spins = 0;

        //singleton
        _instance = this;
    }

    /// <summary>
    /// The private constructor used for cloning
    /// </summary>
    /// <param name="bettingGapSize"></param>
    /// <param name="spinHistory"></param>
    /// <param name="spins"></param>
    private RouletteGame(int bettingGapSize, List<int> spinHistory, int spins) : this(bettingGapSize)
    {
        _spinHistory = spinHistory;
        _spins = spins;
    }

    private void InitializeBoards()
    {
        //Setup the 720 permutations of 123456 using the algorithm encapsulated in the Permutations class
        int[] rgNum = new int[100];
        for (int i = 1; i <= 6; i++)
            rgNum[i] = i;
        Permutations.Permut(1, 6, rgNum);

        //now initialize the board layouts, give them each a layout code which will match the order from the permutations class
        _boardLayouts = new List<BoardLayout>();
        BoardLayout board;

        //if multi board is enabled, permutate the combinations of 123456 on each board
        if (Settings.Default.MULTI_BOARD)
        {
            for (int i = 1; i < 720; i++)
            {
                board = new BoardLayout();
                board.Initialize(_bettingGapSize, (short)i);
                this.Spin += new SpinEventHandler(board.OnSpin);
                _boardLayouts.Add(board);
            }
        }//create one board
        else
        {
            board = new BoardLayout();
            board.Initialize(_bettingGapSize, 1);
            this.Spin += new SpinEventHandler(board.OnSpin);
            _boardLayouts.Add(board);
        }
    }
    /// <summary>
    /// singleton implementation
    /// </summary>
    /// <returns></returns>
    public static RouletteGame GetInstance()
    {
        return _instance;
    }

    public int Spins
    {
        get { return _spins; }
        set { _spins = value; }
    }
    public List<BoardLayout> BoardLayouts
    {
        get { return _boardLayouts; }
        set { _boardLayouts = value; }
    }
    public List<int> SpinHistory
    {
        get { return _spinHistory; }
    }

    /// <summary>
    /// returns the current game strength according to spin numbers
    /// </summary>
    public decimal StrengthIndicator
    {
        get
        {
            decimal ret = (decimal)_spins / 4800;
            ret = ret * 100;
            ret = decimal.Round(ret, 0);
            return ret;
            //return decimal.Round((_spins / 4800)*100, 0);
        }
    }

    /// <summary>
    /// Capture the result of the roulette wheel spin
    /// </summary>
    /// <param name="winningNumber"></param>
    public void CaptureSpin(int winningNumber)
    {

        //increment spins
        _spins++;

        //record history irrespective of number
        _spinHistory.Add(winningNumber);

        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED,
            string.Format("SPIN {0} CHOSE NUMBER {1} ", _spins, winningNumber));

        //publish the spin event
        Spin(this, new SpinEventArgs() { Number = winningNumber });

    }
    /// <summary>
    /// gets the board at the specified layout code
    /// </summary>
    /// <param name="layoutCode"></param>
    /// <returns></returns>
    public BoardLayout GetBoardLayout(int layoutCode)
    {
        return _boardLayouts[layoutCode - 1];
    }

    /// <summary>
    /// Shows the betting summary
    /// </summary>
    /// <returns></returns>
    public string BettingSummary()
    {
        var sb = new StringBuilder();
        short boardCount = 0;
        var spinBets = new List<KeyValuePair<int, decimal>>();
        var ht = new Hashtable();
        foreach (BoardLayout layout in this.BoardLayouts)
        {
            if (layout.BettingActive && layout.BetOnNumbers != null)
            {
                spinBets.Add(new KeyValuePair<int, decimal>(layout.BettingStrategy.SpinBet.Key, layout.BettingStrategy.SpinBet.Value));

                //iterate winning numbers and record counts
                foreach (BoardNumber number in layout.BetOnNumbers)
                {
                    //increment the counter
                    if (ht.ContainsKey(number.Number))
                    {
                        short c = Convert.ToInt16(ht[number.Number]);
                        c++;//:-)
                        ht[number.Number] = c;
                    }
                    else
                        ht.Add(number.Number, 1);
                }
                boardCount++;
            }
        }

        sb.AppendFormat("{0} Total boards were found\n", boardCount);

        var counters = new List<Counter>();
        foreach (DictionaryEntry de in ht)
            counters.Add(new Counter(Convert.ToInt32(de.Key), Convert.ToInt32(de.Value)));
        counters.Sort();

        int counter = 0;
        foreach (Counter c in counters)
        {
            if (c.Count != counter)
            {
                sb.AppendFormat("\n[{0}] occurences of ", c.Count);
            }
            sb.AppendFormat("{0} ", c.NumberValue);

            counter = c.Count;
        }
        sb.AppendLine();
        return sb.ToString();
    }

    public RouletteGame Clone()
    {
        //copy the roulette game data
        var newSpinHistory = new List<int>(_spinHistory.Count);
        newSpinHistory.AddRange(_spinHistory);
        var game = new RouletteGame(_bettingGapSize, newSpinHistory, _spins);

        //copy the board layouts
        var newBoardLayouts = new List<BoardLayout>(_boardLayouts.Count);
        Array.ForEach<BoardLayout>(_boardLayouts.ToArray(), x => newBoardLayouts.Add(x.Clone()));

        return game;
    }
}
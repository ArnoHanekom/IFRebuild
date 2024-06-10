namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class BoardColumn : IComparable<BoardColumn>
{
    int _code;
    List<BoardNumber> _numbers;
    private int _columnWins { get; set; }

    public BoardColumn(int code, List<BoardNumber> numbers)
    {
        _code = code;
        _numbers = numbers;
    }

    public int Code
    {
        get { return _code; }
        set { _code = value; }
    }
    public List<BoardNumber> Numbers
    {
        get { return _numbers; }
        set { _numbers = value; }
    }

    public int ColumnWins => _columnWins;

    /// <summary>
    /// Attempts to find a board number in a specific column
    /// </summary>
    /// <param name="number">The integer to look for</param>
    /// <returns>BoardNumber object reference</returns>
    public BoardNumber FindNumber(int number)
    {
        return _numbers.Find(delegate (BoardNumber bn) { return bn.Number == number; });
    }

    /// <summary>
    /// Attempts to find a board number in this column which contains the passed board code
    /// </summary>
    /// <param name="boardCode">The board code of where the number should be swapped too</param>
    /// <returns>BoardNumber object reference</returns>
    public BoardNumber FindNumberWithCode(int boardCode)
    {
        foreach (BoardNumber number in _numbers)
        {
            if (number.HasBoardCode(boardCode))
            {
                return number;
            }
        }
        return null;
    }

    /// <summary>
    /// Finds a number the contains all the codes contained in the list
    /// </summary>
    /// <param name="boardCodes">the list of codes</param>
    /// <returns>The BoardNumber or (null) if not found</returns>
    public BoardNumber FindNumberWithCodes(List<int> boardCodes)
    {
        foreach (BoardNumber number in _numbers)
        {
            //check for all codes required
            bool contains = true;
            foreach (int code in boardCodes)
            {
                if (!number.HasBoardCode(code))
                {
                    contains = false;
                    break;
                }
            }
            if (contains) return number;
        }
        return null;
    }

    /// <summary>
    /// Returns a list of numbers within a column that match the criteria of codes matching
    /// </summary>
    /// <param name="boardCodes">the board codes the number should contain</param>
    /// <returns>List of BoardNumber</returns>
    public List<BoardNumber> FindNumbersWithCodes(List<int> boardCodes)
    {
        List<BoardNumber> numbers = new List<BoardNumber>();
        foreach (BoardNumber number in _numbers)
        {
            //check for all codes required
            bool contains = true;
            foreach (int code in boardCodes)
            {
                if (!number.HasBoardCode(code))
                {
                    contains = false;
                    break;
                }
            }
            if (contains) numbers.Add(number);
        }
        return numbers;
    }

    public BoardColumn Clone()
    {
        //clone the board numbers into a new array
        var newNumbers = new List<BoardNumber>(_numbers.Count);
        Array.ForEach<BoardNumber>(_numbers.ToArray(), x => newNumbers.Add(x.Clone()));

        //create a new board column object and return
        var boardColumn = new BoardColumn(_code, newNumbers);
        return boardColumn;
    }

    public void AddWin() => ++_columnWins;

    public void ResetWins() => _columnWins = 0;

    #region IComparable<BoardColumn> Members

    public int CompareTo(BoardColumn other)
    {
        return _code.CompareTo(other.Code);
    }

    #endregion
}
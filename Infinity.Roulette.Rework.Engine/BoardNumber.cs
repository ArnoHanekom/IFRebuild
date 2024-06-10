namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class BoardNumber
{
    List<int> _codes;
    int _number;
    int _boardCode;
    bool _immovable;

    public BoardNumber(int number, int boardCode)
    {
        _number = number;
        _boardCode = boardCode;
        _immovable = false;

        //default code layout 
        _codes = new List<int>(6);
        int[] values = { 1, 2, 3, 4, 5, 6 };
        _codes.AddRange(values);
    }

    /// <summary>
    /// The private constructor for cloning purposes
    /// </summary>
    private BoardNumber(int number, int boardCode, List<int> codes)
    {
        _number = number;
        _boardCode = boardCode;
        _codes = codes;
    }

    public List<int> Codes
    {
        get { return _codes; }
        set { _codes = value; }
    }
    public int Number
    {
        get { return _number; }
        set { _number = value; }
    }
    public int BoardCode
    {
        get { return _boardCode; }
        set { _boardCode = value; }
    }
    public bool Immovable
    {
        get { return (bool)(_codes.Count == 1); }
    }

    public void RemoveCode(int code)
    {
        _codes.Remove(code);
    }


    /// <summary>
    /// Returns the Number is [Number] [Codes...] string format
    /// </summary>
    /// <returns>string</returns>
    public override string ToString()
    {
        string s = string.Empty;
        s = string.Format("{0}:{1} ", _number, _boardCode);
        s += "{";
        Array.ForEach<int>(_codes.ToArray(), x => s += x.ToString() + ".");//concatenate the codes into a ".' delimetered string

        if (_codes.Count > 0)
            s = s.Substring(0, s.Length - 1);//remove last "."
        s += "}";
        return s;
    }
    /// <summary>
    /// Checks if the number has a code attached that matches the board code
    /// </summary>
    /// <param name="boardCode"></param>
    /// <returns></returns>
    public bool HasBoardCode(int boardCode)
    {
        int code = _codes.Find(delegate (int i) { return i == boardCode; });

        if (code != 0)
            return true;
        else
            return false;
    }

    public BoardNumber Clone()
    {
        var newCodes = new List<int>(_codes.Count);
        newCodes.AddRange(_codes);

        var boardNumber = new BoardNumber(_number, _boardCode, newCodes);
        return boardNumber;
    }
}
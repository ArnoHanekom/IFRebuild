namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public struct MatrixNumber
{
    int _number;
    bool _win;
    int _column;
    int _row;

    public MatrixNumber(int number, int column, int row)
    {
        _number = number;
        _column = column;
        _row = row;
        _win = false;
    }
    public bool Win
    {
        get { return _win; }
        set { _win = value; }
    }
    public int Column
    {
        get { return _column; }
    }
    public int Row
    {
        get { return _row; }
        set { _row = value; }
    }
    public int Number
    {
        get { return _number; }
    }
    public MatrixNumber Clone()
    {
        var matrixNumber = new MatrixNumber(_number, _column, _row);
        matrixNumber.Win = _win;
        return matrixNumber;
    }
    public override string ToString()
    {
        string s = _number.ToString();
        if (_win)
            s += "w";
        return s;
    }
}
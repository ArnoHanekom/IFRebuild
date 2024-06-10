namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class BettingStrategy
{
    int _gapSize;
    Gap _currentGap;
    short _cursor;

    public BettingStrategy(int gapSize)
    {
        _gapSize = gapSize;
        _currentGap = GapFactory.GetNewGap(gapSize);
        _cursor = 0;
    }
    private BettingStrategy(int gapSize, Gap currentGap, short cursor)
    {
        _gapSize = gapSize;
        _currentGap = currentGap;
        _cursor = cursor;
    }
    public int GapSize
    {
        get { return _gapSize; }
        set { _gapSize = value; _currentGap.GapSize = _gapSize; _cursor = 0; }
    }
    public KeyValuePair<int, decimal> SpinBet
    {
        get { return _currentGap.SpinBets[_cursor]; }
    }
    public Gap CurrentGap
    {
        get { return _currentGap; }
    }
    public void Spin(bool win)
    {
        //start the cursor from the first row again if spin size reached. My world starts at 0, yours?
        //also a registerd win in the board resets the counter in the betting strategy
        if (_cursor == _currentGap.SpinSize - 1 || win == true)
            _cursor = 0;
        else
            _cursor++;
    }

    #region ICloneable Members

    public BettingStrategy Clone()
    {
        var bettingStrategy = new BettingStrategy(_gapSize, _currentGap, _cursor);
        return bettingStrategy;
    }

    #endregion
}
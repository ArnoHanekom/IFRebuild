namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class Gap
{
    //this is the constant used to define the bet increment sequence depending on the Gap Size
    int[] BET_INCREMENT_SEQUENCE =
        new int[] { 1, 1, 1, 1, 2, 2, 2, 3, 3, 4, 5, 6, 7, 8, 10, 12, 15, 18, 22, 25, 30, 36, 44, 52, 64, 76, 92, 110, 132, 158, 200, 230, 280, 335, 400 };

    int _gapSize;
    decimal _betSize;
    int _spinSize;

    List<KeyValuePair<int, decimal>> _spinBets;

    public Gap(int gapSize, decimal betSize, int spinSize)
    {
        _gapSize = gapSize;
        _betSize = betSize;
        _spinSize = spinSize;

        InitializeSpinBets();
    }

    public List<KeyValuePair<int, decimal>> SpinBets
    {
        get { return _spinBets; }
    }
    public int GapSize
    {
        get { return _gapSize; }
        set
        {
            _gapSize = value;
            Gap g = GapFactory.GetNewGap(_gapSize);
            _betSize = g.BetSize;
            _spinSize = g.SpinSize;
            InitializeSpinBets();
            g = null;
        }
    }
    public decimal BetSize
    {
        get { return _betSize; }
    }
    public int SpinSize
    {
        get { return _spinSize; }
    }
    private void InitializeSpinBets()
    {
        _spinBets = new List<KeyValuePair<int, decimal>>();
        //ok initialize the spin bets
        for (int i = 1; i <= _spinSize; i++)
        {
            var spinBet = new KeyValuePair<int, decimal>(i,
                BET_INCREMENT_SEQUENCE[i - 1] * _betSize);
            _spinBets.Add(spinBet);
        }
    }
}


public class GapFactory
{
    public static Gap GetNewGap(int gapSize)
    {
        decimal bet;
        int spinSize;
        switch (gapSize)
        {
            case 54:
                bet = 1; spinSize = 35;
                break;
            case 60:
                bet = 2; spinSize = 31;
                break;
            case 66:
                bet = 5; spinSize = 26;
                break;
            case 72:
                bet = 10; spinSize = 22;
                break;
            case 78:
                bet = 25; spinSize = 17;
                break;
            case 84:
                bet = 50; spinSize = 14;
                break;
            case 90:
                bet = 100; spinSize = 10;
                break;
            case 96:
                bet = 100; spinSize = 10;
                break;
            case 102:
                bet = 100; spinSize = 10;
                break;
            case 108:
                bet = 200; spinSize = 7;
                break;
            default: bet = 1; spinSize = 35; break;
        }
        return new Gap(gapSize, bet, spinSize);
    }
}
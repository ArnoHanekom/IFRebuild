namespace Infinity.Roulette.Rework.Engine;

public class GameHistory
{
    public RouletteGame StateData { get; set; }
    public int SpinNumber { get; set; }

    public GameHistory(int spinNumber, RouletteGame stateData)
    {
        StateData = stateData;
        SpinNumber = spinNumber;
    }
}
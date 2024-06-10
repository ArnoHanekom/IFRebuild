namespace Infinity.Roulette.Rework.Engine;

public interface IBoardSummary
{
    int Gap { get; set; }
    decimal Bet { get; set; }
    int Number1 { get; set; }
    int Number2 { get; set; }
    int Number3 { get; set; }
    int Number4 { get; set; }
    int Number5 { get; set; }
    int Number6 { get; set; }
}

[Serializable()]
public class BoardSummary : IBoardSummary, IComparable<BoardSummary>
{
    public int BoardNo { get; set; }
    public int Gap { get; set; }
    public decimal Bet { get; set; }
    public int Number1 { get; set; }
    public int Number2 { get; set; }
    public int Number3 { get; set; }
    public int Number4 { get; set; }
    public int Number5 { get; set; }
    public int Number6 { get; set; }

    #region IComparable<IBoardSummary> Members

    public int CompareTo(BoardSummary other)
    {
        return Gap.CompareTo(other.Gap);
    }

    #endregion
}

[Serializable()]
public class BoardModel
{
    public List<IBoardSummary> BoardSummaries { get; set; }
}
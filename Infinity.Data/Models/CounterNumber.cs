namespace Infinity.Data.Models;

public class CounterNumber
{
    public CounterNumber(int num, object style, Guid table, int spinNo, bool boardReset, int spinType)
    {
        Number = num;
        TextBlockStyle = style;
        GameTable = table;
        SpinNo = spinNo;
        BoardReset = boardReset;
        SpinType = spinType;
    }
    public int Number { get; init; }
    public object TextBlockStyle { get; set; } = default!;
    public Guid GameTable { get; init; }
    public int SpinNo { get; init; }
    public bool BoardReset { get; init; }
    public int SpinType { get; init; }
}
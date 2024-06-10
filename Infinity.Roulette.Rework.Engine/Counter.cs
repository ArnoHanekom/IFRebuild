namespace Infinity.Roulette.Rework.Engine;

/// <summary>
/// Used for counting buckets of numbers in the array and then sorting them. 
/// Yes i hear you there is prob a better way, but its late. peace. 
/// </summary>
[Serializable()]
public class Counter : IComparable<Counter>
{
    public Counter(int numberValue, int count)
    {
        NumberValue = numberValue;
        Count = count;
    }

    public int NumberValue { get; set; }
    public int Count { get; set; }

    public Counter Clone()
    {
        var counter = new Counter(NumberValue, Count);
        return counter;
    }

    #region IComparable<Counter> Members

    public int CompareTo(Counter other)
    {
        return other.Count.CompareTo(Count);
    }
    #endregion
}
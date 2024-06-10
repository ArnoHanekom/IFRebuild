using Infinity.Data.Models;
using Infinity.Services.Interfaces;

namespace Infinity.Services.Services;

public class CountLabelService : ICountLabelService
{
    private List<CounterNumber> countNumbers = new();
    private List<CounterNumber> prevSpinCountNumbers = new();
    public void AddCountNumber(CounterNumber cn)
    {
        lock (countNumbers)
        {
            countNumbers.Add(cn);
        }
    }

    public void AddPrevSpinCountNumber(CounterNumber cn)
    {
        lock (prevSpinCountNumbers)
        {
            prevSpinCountNumbers.Add(cn);
        }
    }

    public void AddPrevSpinCountNumbers(List<CounterNumber> numbers)
    {
        lock (prevSpinCountNumbers) 
        {
            prevSpinCountNumbers.AddRange(numbers);    
        }
    }

    public CounterNumber[] GetCountNumbers(Guid tableId)
    {
        lock (countNumbers)
        {
            return countNumbers.Where(cn => cn.GameTable == tableId) .ToArray();
        }
    }
    public CounterNumber[] GetPreviousSpinCountNumbers(Guid tableId)
    {
        lock (prevSpinCountNumbers)
        {
            return prevSpinCountNumbers.Where(pcn => pcn.GameTable == tableId).ToArray();
        }
    }

    public void ClearCountNumbers()
    {
        lock (countNumbers)
        {
            countNumbers = new();
        }
    }

    public void ClearPrevSpinCountNumbers()
    { 
        lock (prevSpinCountNumbers)
        {
            prevSpinCountNumbers = new();
        }
    }

    public void UpdateCountNumberStyle(Guid tableId, int number, object newStyle)
    {
        lock (countNumbers) 
        {            
            foreach (var cNum in countNumbers)
            {
                if (cNum.Number == number && cNum.GameTable == tableId)
                {
                    cNum.TextBlockStyle = newStyle;
                }
            }
        }
    }
}
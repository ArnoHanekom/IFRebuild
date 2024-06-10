using Infinity.Data.Models;

namespace Infinity.Services.Interfaces;

public interface ICountLabelService
{
    public void AddCountNumber(CounterNumber cn);
    public CounterNumber[] GetCountNumbers(Guid tableId);
    public void ClearCountNumbers();
    public void AddPrevSpinCountNumber(CounterNumber cn);
    public void AddPrevSpinCountNumbers(List<CounterNumber> numbers);
    public CounterNumber[] GetPreviousSpinCountNumbers(Guid tableId);
    public void ClearPrevSpinCountNumbers();
    public void UpdateCountNumberStyle(Guid tableId, int number, object newStyle);
}
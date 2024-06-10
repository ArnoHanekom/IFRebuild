using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.Rework.Data.Models;

public class Spin
{
    private readonly object _lock = new();

    public Guid Id { get; init; }
    public int SpinNr { get; set; }
    public int WinNumber { get; set; }
    public SpinType Type { get; set; }

    private List<CountNumber> _countNumbers { get; set; }

    public CountNumber[] CountNumbers
    {
        get
        {
            lock (_lock)
            {
                return _countNumbers.ToArray();
            }
        }
        set
        {
            _countNumbers = value.ToList();
        }
    }

    public Spin()
    {
        Id = Guid.NewGuid();
        Type = SpinType.None;
        _countNumbers = new();
    }

    public void AddCountNumber(CountNumber countNumber)
    {
        lock (_lock)
        {
            _countNumbers.Add(countNumber);
        }
    }
    public void SetCountNumbers(List<CountNumber> countNumbers)
    {
        lock (_lock)
        {
            _countNumbers = countNumbers;
        }
    }
}
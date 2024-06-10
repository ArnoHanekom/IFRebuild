using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.Rework.Data.Models;

public class Phase
{
    private readonly object _lock = new();

    public Guid Id { get; init; }
    public SpinType SpinType { get; set; } = SpinType.None;
    public bool Started { get; set; } = true;
    public bool Ended { get; set; } = false;
    public long StartedAt { get; set; } = DateTime.Now.Ticks;
    public long? EndedAt { get; set; }

    private List<Spin> _spins { get; set; }

    public Spin[] Spins
    {
        get
        {
            lock (_lock)
            {
                return _spins.ToArray();
            }
        }
        set
        {
            _spins = value.ToList();
        }
    }

    public Phase()
    {
        Id = Guid.NewGuid();
        _spins = new();
    }
    public void AddSpin(Spin spin)
    {
        lock (_lock) 
        { 
            _spins.Add(spin);
        }
    }
    public void SetSpins(List<Spin> spins)
    {
        lock (_lock)
        {
            _spins = spins;
        }
    }
}
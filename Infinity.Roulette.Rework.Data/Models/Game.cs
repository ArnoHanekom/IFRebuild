using Infinity.Roulette.Rework.Engine;

namespace Infinity.Roulette.Rework.Data.Models;

public class Game
{
    private readonly object _lock = new();
    private List<Phase> _phases { get; set; }

    public Guid Id { get; init; }
    public Phase[] Phases
    {
        get
        {
            lock (_lock)
            {
                return _phases.ToArray();
            }
        }
        set
        {
            _phases = value.ToList();
        }
    }
    
    private RouletteGame _roulette { get; set; }
    public RouletteGame Roulette
    {
        get
        {
            lock ( _lock)
            {
                return _roulette;
            }
        }
        set
        {
            _roulette = value;
        }
    }

    public Game()
    {
        Id = Guid.NewGuid();
        _phases = new();
        _roulette = RouletteGame.CreateGame(36);
    }

    public void AddPhase(Phase phase)
    {
        lock(_lock)
        {
            _phases.Add(phase);
        }
    }
    public void SetPhases(List<Phase> phases)
    {
        lock (_lock)
        {
            _phases = phases;
        }
    }

    public void ResetRoulette()
    {
        lock (_lock)
        {
            this._roulette = RouletteGame.CreateGame(36);
        }
    }
}
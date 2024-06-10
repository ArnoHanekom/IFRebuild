using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Engine.Services
{
    public class EngineService : IEngineService
    {
        private readonly object _lock = new();
        private readonly object _resetLock = new();
        private readonly object _spinLock = new();
        private readonly object _phaseSpinLock = new();

        private ConcurrentBag<GamePhase> _gamePhases = new();
        private ConcurrentBag<GameReset> _gameResets = new();
        private ConcurrentBag<SpinEntity> _spins = new();
        private ConcurrentBag<PhaseSpin> _phaseSpins = new();

        public async Task NewGamePhaseListAsync()
        {
            await Task.Run(() =>         
            {
                lock (_lock)
                {
                    _gamePhases = new();
                }
            });
        }

        public async Task AddGamePhaseAsync(Guid tableId, Guid gameId, int phaseType, int spinType)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    var openPhaseTypes = _gamePhases.Where(gp => gp.TableId == tableId && gp.GameId == gameId && gp.PhaseType == phaseType && gp.EndType == 0).Count();
                    if (openPhaseTypes == 0)
                    {
                        var phase = new GamePhase(tableId, gameId, phaseType, spinType);
                        _gamePhases.Add(phase);
                    }
                }
            });
        }

        public async Task UpdatePhaseEndAsync(Guid tableId, Guid gameId, int endPhase)
        {
            await Task.Run(() =>
            { 
                lock (_lock)
                {
                    var phase = _gamePhases.FirstOrDefault(gp => gp.TableId == tableId && gp.GameId == gameId && gp.EndType == 0);
                    if (phase is not null)
                    {
                        phase.EndType = endPhase;
                    }
                }
            });
        }

        public async Task UpdatePhaseDrawnAsync(Guid tableId, Guid gameId)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    var phases = _gamePhases.Where(gp => gp.TableId == tableId && gp.GameId == gameId && gp.Drawn == false);
                    foreach (var phase in phases)
                    {
                        phase.Drawn = true;
                    }
                }
            });
        }
        public async Task IncreaseResetCountAsync(Guid tableId, Guid gameId)
        {
            await Task.Run(() =>
            {
                lock (_resetLock)
                {
                    var gameReset = _gameResets.FirstOrDefault(gr => gr.TableId == tableId && gr.GameId == gameId);
                    if (gameReset is null)
                    {
                        gameReset = new GameReset()
                        {
                            TableId = tableId,
                            GameId = gameId
                        };

                        _gameResets.Add(gameReset);
                    }
                    else
                    {
                        gameReset.ResetCount++;
                    }
                }
            });
        }
        public async Task<int> GetGameResetCountAsync(Guid tableId, Guid gameId)
        {
            return await Task.Run(() =>
            {
                lock (_resetLock)
                {
                    var gameReset = _gameResets.FirstOrDefault(gr => gr.TableId == tableId && gr.GameId == gameId);
                    return gameReset?.ResetCount ?? 0;
                }
            });
        }

        public async Task WritePhasesAsync()
        {
            await Task.Run(() =>
            {
                lock (_resetLock)
                {
                    var phases = _gamePhases.OrderBy(gr => gr.TableId).ThenBy(gr => gr.GameId);
                    Debug.WriteLine("+++++++++++  PHASES  ++++++++++++");
                    foreach (var phase in phases)
                    {
                        Debug.WriteLine($"Table: {phase.TableId}\tGame: {phase.GameId}\tType: {phase.PhaseType}\tStart Spin: {phase.StartType}\tEnd Spin: {phase.EndType}");
                        Task.Run(async()=>
                        {
                            var resetCount = await GetGameResetCountAsync(phase.TableId, phase.GameId);
                            Debug.WriteLine($"Resets: {resetCount}");
                        });                        
                        Debug.WriteLine($"");
                    }

                    Debug.WriteLine("++++++++++++++++++++++++++++++++");
                }
            });
        }

        public async Task CompileTableSpinsIfnoAsync()
        {
            
            await Task.Run(() =>
            {
                lock (_resetLock)
                {
                    Debug.WriteLine("+++++++++++  PHASES  ++++++++++++");
                    var tablesAndGames = _gamePhases.Select(gr => new { gr.TableId, gr.GameId }).Distinct();
                    lock (tablesAndGames)
                    {
                        foreach (var tableAndGame in tablesAndGames)
                        {
                            var resetCount = GetGameResetCount(tableAndGame.TableId, tableAndGame.GameId);
                            Debug.WriteLine($"Table: {tableAndGame.TableId}\tGame: {tableAndGame.GameId}\tResets: {resetCount}");
                            Debug.WriteLine($"Phases:");
                            var tgPhases = _gamePhases.Where(gp => gp.TableId == tableAndGame.TableId);
                            foreach (var tgphase in tgPhases)
                            {
                                Debug.WriteLine($"Type: {tgphase.PhaseType}\tStart Spin: {tgphase.StartType}\tEnd Spin: {tgphase.EndType}");
                            }

                            Debug.WriteLine($"");
                        }
                    }
                    Debug.WriteLine("++++++++++++++++++++++++++++++++");
                }
            });
        }

        public int GetGameResetCount(Guid tableId, Guid gameId)
        {
            lock (_resetLock)
            {
                var gameReset = _gameResets.FirstOrDefault(gr => gr.TableId == tableId && gr.GameId == gameId);
                return gameReset?.ResetCount ?? 0;
            }
        }

        public async Task<GamePhase?> GetLastOpenUndrawnPhase(Guid tableId, Guid gameId)
        {
            return await Task.Run(() => 
            {
                lock (_resetLock)
                {
                    lock (_gamePhases)
                    {
                        var phase = _gamePhases.LastOrDefault(gp => gp.TableId == tableId && gp.GameId == gameId && gp.EndType == 0 && gp.Drawn == false);
                        return phase;
                    }
                }
            });
        }

        public async Task<GamePhase?> GetLastOpenPhase(Guid tableId, Guid gameId)
        {
            return await Task.Run(() =>
            {
                lock (_resetLock)
                {
                    lock (_gamePhases)
                    {                        
                        var phase = _gamePhases.LastOrDefault(gp => gp.TableId == tableId && gp.GameId == gameId && gp.EndType == 0);
                        return phase;
                    }
                }
            });
        }

        public async Task<List<GamePhase>> GetAllPhasesAsync(Guid tableId, Guid gameId)
        {
            return await Task.Run(() =>
            {
                lock (_resetLock)
                {
                    return _gamePhases.Where(gp => gp.TableId == tableId && gp.GameId == gameId).ToList();
                }
            });
        }

        public async Task AddSpinEntityAsync(SpinEntity spinEntity)
        {
            await Task.Run(() =>
            {
                lock (_spinLock)
                {
                    _spins.Add(spinEntity);
                }
            });
        }
        public async Task<List<SpinEntity>> GetSpinEntitiesAsync(Guid tableId, Guid gameId)
        {
            return await Task.Run(() =>
            {
                lock (_spinLock)
                {
                    lock (_spins)
                    {
                        var gid = Guid.Parse("fc43d207-cb7a-4144-b4ff-db3ddc40ef42");

                        return _spins.Where(s => s.TableId == tableId && s.GameId == gameId).ToList();
                    }
                }
            });
        }
        public async Task AddUpdatePhaseSpin(Guid phaseId, Guid spinId)
        {
            await Task.Run(() =>
            {
                lock (_phaseSpinLock)
                {
                    lock (_phaseSpins)
                    {
                        var phase = _phaseSpins.FirstOrDefault(ps => ps.PhaseId == phaseId);
                        if (phase is null)
                        {
                            phase = new PhaseSpin() { PhaseId = phaseId, SpinId = spinId };
                            _phaseSpins.Add(phase);
                        }
                        else
                        {
                            phase.SpinId = spinId;
                        }
                    }
                }
            });
        }

        public async Task<PhaseSpin?> GetLastPhaseSpin(Guid phaseId)
        {
            return await Task.Run(() =>
            {
                lock (_phaseSpinLock)
                {
                    lock (_phaseSpins)
                    {
                        var phase = _phaseSpins.FirstOrDefault(ps => ps.PhaseId == phaseId);
                        return phase;
                    }
                }
            });
        }

    }
}

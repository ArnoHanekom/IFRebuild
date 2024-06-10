using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Engine.Services
{
    public interface IEngineService
    {
        public Task NewGamePhaseListAsync();
        public Task AddGamePhaseAsync(Guid tableId, Guid gameId, int phaseType, int spinType);
        public Task UpdatePhaseEndAsync(Guid tableId, Guid gameId, int endPhase);
        public Task UpdatePhaseDrawnAsync(Guid tableId, Guid gameId);
        public Task IncreaseResetCountAsync(Guid tableId, Guid gameId);
        public Task<int> GetGameResetCountAsync(Guid tableId, Guid gameId);
        public int GetGameResetCount(Guid tableId, Guid gameId);
        public Task WritePhasesAsync();
        public Task CompileTableSpinsIfnoAsync();
        public Task<GamePhase?> GetLastOpenUndrawnPhase(Guid tableId, Guid gameId);
        public Task<GamePhase?> GetLastOpenPhase(Guid tableId, Guid gameId);
        public Task<List<GamePhase>> GetAllPhasesAsync(Guid tableId, Guid gameId);
        public Task AddSpinEntityAsync(SpinEntity spinEntity);
        public Task<List<SpinEntity>> GetSpinEntitiesAsync(Guid tableId, Guid gameId);
        public Task AddUpdatePhaseSpin(Guid phaseId, Guid spinId);
        public Task<PhaseSpin?> GetLastPhaseSpin(Guid phaseId);
    }
}

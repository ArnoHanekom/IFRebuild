using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Engine
{
    public class GamePhase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SpinId { get; set; }
        public Guid TableId { get; set; }
        public Guid GameId { get; set; }
        public int PhaseType { get; set; } = 0;
        public int StartType { get; set; } = 0;
        public int EndType { get; set; } = 0;
        public bool Drawn { get; set; } = false;

        public GamePhase()
        {
            
        }

        public GamePhase(Guid tableId, Guid gameId, int phaseType, int startType)
        {
            TableId = tableId;
            GameId = gameId;
            PhaseType = phaseType;
            StartType = startType;
            EndType = 0;
            Drawn = false;
        }        
    }
}

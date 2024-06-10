using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Engine
{
    public class GameReset
    {
        public Guid TableId { get; set; }
        public Guid GameId { get; set; }
        public int ResetCount { get; set; } = 0;
    }
}

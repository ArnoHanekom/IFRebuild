using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Engine
{
    public class SpinEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TableId { get; set; }
        public Guid GameId { get; set; }
        public int WinningNumber { get; set; }
        public int Type { get; set; }
        public List<CountNumber> CountNumbers { get; set; } = new();
    }
}

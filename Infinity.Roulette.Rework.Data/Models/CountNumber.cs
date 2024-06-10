using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.Rework.Data.Models;

public class CountNumber
{
    public int Number { get; set; }
    public int BoardCode { get; set; }
    public CountColor Color { get; set; } = CountColor.None;
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.Rework.Data.Models;

public class Autoplay
{
    private readonly object _lock = new();
    public Guid Id { get; init; }

    private List<GameTable> _tables { get; set; }
    public GameTable[] Tables
    {
        get
        {
            lock (_lock) 
            {
                return _tables.ToArray();
            }            
        }
    }

    public Autoplay()
    {
        Id = Guid.NewGuid();
        _tables = new();
    }

    public void AddTable(GameTable table)
    {
        lock (_lock) 
        {
            _tables.Add(table);
        }        
    }
    public void SetTables(List<GameTable> tables)
    {
        lock (_lock)
        {
            _tables = tables;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.Rework.Data.Models;

public class History
{
    private readonly object _lock = new();
    public Guid Id { get; init; }

    private List<string> _tablesJson { get; set; }
    public string[] TablesJson
    {
        get
        {
            lock (_lock)
            {
                return _tablesJson.ToArray();
            }
        }
    }

    public History()
    {
        Id = Guid.NewGuid();
        _tablesJson = new();
    }

    public void AddTableJson(string json)
    {
        lock (_lock)
        {
            _tablesJson.Add(json);
        }
    }

    public void SetTablesJson(List<string> tablesJson)
    {
        lock (_lock)
        {
            _tablesJson = tablesJson;
        }
    }
}
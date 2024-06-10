// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.LimitService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Constants;
using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Services.Services
{
  public class LimitService : ILimitService
  {
    private List<Limit> _limits { get; set; } = new List<Limit>();

    public Limit CreateLimit(LimitType type, int value) => new Limit()
    {
      Type = type,
      Value = value
    };

    public List<Limit> AddToList(Limit limit)
    {
      _limits.Add(limit);
      return _limits;
    }

    public List<Limit> Get() => _limits;

    public void ChangeLimit(LimitType type, int value)
    {
      Limit limit = _limits.FirstOrDefault(l => l.Type == type)!;
      if (limit == null)
        return;
      _limits[_limits.IndexOf(limit)].Value = value;
    }
  }
}

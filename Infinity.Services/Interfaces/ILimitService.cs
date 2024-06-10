﻿// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Interfaces.ILimitService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Constants;
using Infinity.Data.Models;
using System.Collections.Generic;


#nullable enable
namespace Infinity.Services.Interfaces
{
  public interface ILimitService
  {
    Limit CreateLimit(LimitType type, int value);

    List<Limit> AddToList(Limit limit);

    List<Limit> Get();
  }
}

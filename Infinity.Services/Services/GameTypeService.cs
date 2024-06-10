// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.GameTypeService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Constants;
using Infinity.Services.Interfaces;
using System;
using System.Collections.Generic;


#nullable enable
namespace Infinity.Services.Services
{
  public class GameTypeService : IGameTypeService
  {
    public GameType Get(string gameType) => (GameType) Enum.Parse(typeof (GameType), gameType);

    public List<GameType> GetTypes()
    {
      List<GameType> types = new List<GameType>();
      foreach (GameType gameType in Enum.GetValues(typeof (GameType)))
      {
        switch (gameType)
        {
          case GameType.Spinfile:
          case GameType.Manual:
            continue;
          default:
            types.Add(gameType);
            continue;
        }
      }
      return types;
    }
  }
}

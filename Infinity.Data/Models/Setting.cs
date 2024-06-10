// Decompiled with JetBrains decompiler
// Type: Infinity.Data.Models.Setting
// Assembly: Infinity.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A2C88E43-EACF-400C-AB56-5967E8E08F14
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Data.dll

using Infinity.Data.Constants;
using System.Collections.Generic;


#nullable enable
namespace Infinity.Data.Models
{
  public class Setting
  {
    public GameType DefaultGameType { get; set; }

    public List<GameSetting> GameSettings { get; set; } = new List<GameSetting>();

    public string DefaultGameTypeStr => DefaultGameType.ToString();
  }
}

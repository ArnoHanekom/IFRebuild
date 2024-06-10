// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.SettingService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;


#nullable enable
namespace Infinity.Services.Services
{
  public class SettingService : ISettingService
  {
    private Setting? _setting { get; set; }

    private Setting? _dashSetting { get; set; }

    public void New() => _setting = new Setting();

    public void NewDashSetting() => _dashSetting = new Setting();

    public Setting Get() => _setting ?? new Setting();

    public Setting GetDashSetting() => _dashSetting ?? new Setting();

    public void Save(Setting setting) => _setting = setting;

    public void SaveDashSetting(Setting setting) => _dashSetting = setting;
  }
}

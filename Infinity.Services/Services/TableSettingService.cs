// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.TableSettingService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;


#nullable enable
namespace Infinity.Services.Services
{
  public class TableSettingService : ITableSettingService
  {
    private TableSetting? _setting { get; set; }

    private TableSetting? _dashSetting { get; set; }

    public void New() => _setting = new TableSetting();

    public void NewDashSetting() => _dashSetting = new TableSetting();

    public TableSetting Get() => _setting ?? new TableSetting();

    public TableSetting GetDashSetting() => _dashSetting ?? new TableSetting();

    public void Save(TableSetting setting) => _setting = setting;

    public void SaveDashSetting(TableSetting setting) => _dashSetting = setting;
  }
}

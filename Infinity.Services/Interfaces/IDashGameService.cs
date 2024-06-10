// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Interfaces.IDashGameService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using System.Threading.Tasks;


#nullable enable
namespace Infinity.Services.Interfaces
{
  public interface IDashGameService
  {
    void NewDashGame();

    Task<Dashboard> GetAsync();

    void Save(Dashboard dashGame);

    Task<Dashboard> PopulateDashNewTablesAsync();

    Task<Dashboard> PopulateDashNewTablesAsync(Dashboard dashy);

    Task<double> GetDashGamSpinPercentageAsync();

    Task<Table> GetPlayTable(int tableId);

    Task<Dashboard> UpdateDashGameAsync(Table table);
  }
}

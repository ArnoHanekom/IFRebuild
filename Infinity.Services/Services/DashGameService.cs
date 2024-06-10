// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.DashGameService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Infinity.Services.Services
{
  public class DashGameService : IDashGameService
  {
    private Dashboard _dashGame;

    public DashGameService() => _dashGame = new Dashboard();

    public void NewDashGame() => _dashGame = new Dashboard();

    public async Task<Dashboard> GetAsync() => await Task.Run(() => _dashGame);

    public void Save(Dashboard dashGame) => _dashGame = dashGame;

    public async Task<Dashboard> PopulateDashNewTablesAsync() => await Task.Run(() => PopulateDashNewTables());

    public async Task<Dashboard> PopulateDashNewTablesAsync(Dashboard dashy)
    {
            _dashGame = dashy;
            return await Task.Run(() => PopulateDashNewTables());
        }

    private Dashboard PopulateDashNewTables()
    {
      Dashboard dashGame = _dashGame;
      //for (int index = 0; index < dashGame.PlayTables.Length; ++index)
        
      return dashGame;
    }

    public async Task<double> GetDashGamSpinPercentageAsync() => await Task.Run(() => GetDashGamSpinPercentage());

    private double GetDashGamSpinPercentage() => _dashGame.SpinPercentage;

    public async Task<Table> GetPlayTable(int tableId) => await Task.Run(() =>
    {
        lock (_dashGame)
            return _dashGame.PlayTables[tableId];
    });

    public async Task<Dashboard> UpdateDashGameAsync(Table table) => await Task.Run(() =>
    {
        lock (_dashGame)
        {
            _dashGame.PlayTables[table.TableId] = table;
            return _dashGame;
        }
    });
  }
}

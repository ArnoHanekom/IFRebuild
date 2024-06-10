// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.SpinWatcherService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;


#nullable enable
namespace Infinity.Services.Services
{
  public class SpinWatcherService : ISpinWatcherService
  {
    private SpinWatcher _watcher { get; set; } = default!;

    public void NewWatcher() => _watcher = new SpinWatcher();

    public void IncreaseSpin(int spins) => _watcher.CurrentSpin += spins;

    public void SetTotal(int totalSpins) => _watcher.TotalSpins = totalSpins;

    public double ProgressPercentage => _watcher.SpinProgress;
  }
}

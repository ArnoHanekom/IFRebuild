// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Interfaces.ITableService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using System.Collections.Generic;


#nullable enable
namespace Infinity.Services.Interfaces
{
  public interface ITableService
  {
    void NewPlaySearch();

    void AddTable(Table table);

    Table? Get(int tableId, int autoplay);

    List<Table> Get();

    void SetTotalCalculatedSpins(int calcSpins);

    int GetTotalCalculatedSpins();

    void AddOverallSpin();

    int GetCurrentOverallSpins();

    double GetCurrentPercentage();

    void ClearPlaySearch();

    void ResetCounters();

    void AddDoneSpins(int remainingCount);
  }
}

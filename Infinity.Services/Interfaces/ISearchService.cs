// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Interfaces.ISearchService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Infinity.Services.Interfaces
{
  public interface ISearchService
  {
    void NewSearch();

    Task NewSearchAsync();

    Task<IEnumerable<Table>> GetAsync();

    Task AddResult(Table result);

    Task<Table?> Get(int tableId, int autoplayNumber);

    Task<IEnumerable<Table>> GetResults();

    Task<int> ExactMatchCount();

    int GetExactMatchCount();

    int GetSpinResultsExactMatchCount();

    int GetSpinResultsR1WMatchCount();

    int GetSpinResultsTWMatchCount();

    void NewSpinSearch();

    List<Table> GetSpinResults();

    void AddSpinResult(Table table);

    Table? GetSpinResultTable(int tableId, int autoplay);

    IEnumerable<Table> GetAllSpinResults();
  }
}

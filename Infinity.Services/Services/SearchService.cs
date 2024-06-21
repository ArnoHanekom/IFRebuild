// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.SearchService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Infinity.Services.Services
{
    public class SearchService : ISearchService
    {
        private IEnumerable<Table> _results { get; set; } = default!;

        private IEnumerable<Table> _searchResults { get; set; } = default!;

        private List<Table> _spinResults { get; set; } = default!;

        public void NewSearch() => _results = new HashSet<Table>();

        public void NewSpinSearch() => _spinResults = new List<Table>();

        public async Task NewSearchAsync()
        {
            await Task.Run(() => _results = new HashSet<Table>());
        }

        public async Task<IEnumerable<Table>> GetAsync() => await Task.Run(() => _results);


        public List<Table> GetSpinResults()
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                return _spinResults;
            }
        }

        public void AddSpinResult(Table table)
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                if (!_spinResults.Any(sr => sr.TableId == table.TableId && sr.Autoplay == table.Autoplay))
                {
                    _spinResults.Add(table);
                }
            }
        }

        public Table? GetSpinResultTable(int tableId, int autoplay)
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                return _spinResults.FirstOrDefault(sr => sr.TableId == tableId && sr.Autoplay == autoplay);
            }
        }

        public IEnumerable<Table> GetAllSpinResults()
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                return _spinResults.OrderByDescending(sr => sr.Rows);
            }
        }

        public async Task AddResult(Table result) => await Task.Run(() =>
        {
            lock (_results)
            {
                if (_results.Any(r => r.TableId == result.TableId && r.Autoplay == result.Autoplay))
                    return;
                List<Table> resultList = _results.ToList();
                resultList.Add(result);
                _results = resultList;
            }
        });

        public async Task<Table?> Get(int tableId, int autoplayNumber)
        {
            return await Task.Run(() =>
            {
                lock (_results) { return _results.FirstOrDefault(r => r.TableId == tableId && r.Autoplay == autoplayNumber); }
            });
        }

        public async Task<IEnumerable<Table>> GetResults()
        {
            _searchResults = _results;
            return await Task.Run(() => _searchResults.OrderByDescending(r => r.Rows));
        }

        public async Task<int> ExactMatchCount() => await Task.Run(() =>
        {
            lock (_results) { return _results.Count(r => r.ExactMatch && r.DoneSpinning); }
        });

        public int GetExactMatchCount()
        {
            lock (_searchResults)
            {
                return _searchResults.Count(sr => sr.ExactMatch);
            }
        }

        public int GetSpinResultsExactMatchCount()
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                return _spinResults.Count(st => st.ExactMatch && st.DoneSpinning);
            };
        }

        public int GetSpinResultsR1WMatchCount()
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                return _spinResults.Count(st => st.R1WMatch && st.isHighestRowWin && st.DoneSpinning && st.WinsMatch);
            };
        }

        public int GetSpinResultsTWMatchCount()
        {
            _spinResults ??= new();
            lock (_spinResults)
            {
                return _spinResults.Count(st => st.TWMatch && st.DoneSpinning && st.WinsMatch);
            };
        }
    }

    public static class AsyncExtensions
    {
        public static async Task<int> ToExactMatchCount(this IAsyncEnumerable<Table> items)
        {
            int exactMatches = 0;
            await foreach (var item in items)
            {
                if (item.DoneSpinning && item.ExactMatch) exactMatches += 1;
            }
            return exactMatches;
        }
    }   
}

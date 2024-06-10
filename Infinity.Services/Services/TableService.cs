﻿// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.TableService
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Infinity.Services.Services
{
    public class TableService : ITableService
    {
        private List<Table> spinTables { get; set; } = new();

        private int TotalCalculatedSpins { get; set; }

        private int CurrentOverallSpins { get; set; }

        public void NewPlaySearch() => spinTables = new List<Table>();

        public void ClearPlaySearch()
        {
            lock (spinTables)
                spinTables = new List<Table>();
        }

        public void AddTable(Table table)
        {
            if (spinTables == null)
            {
                List<Table> tableList;
                spinTables = tableList = new List<Table>();
            }
            lock (spinTables)
            {
                if (spinTables.Any(st => st.TableId == table.TableId && st.Autoplay == table.Autoplay))
                    return;
                spinTables.Add(table);
            }
        }

        public Table? Get(int tableId, int autoplay)
        {
            if (spinTables == null)
            {
                List<Table> tableList;
                spinTables = tableList = new List<Table>();
            }
            lock (spinTables)
                return spinTables.FirstOrDefault(st => st.TableId == tableId && st.Autoplay == autoplay);
        }

        public List<Table> Get()
        {
            if (spinTables == null)
            {
                List<Table> tableList;
                spinTables = tableList = new List<Table>();
            }
            lock (spinTables)
                return spinTables.OrderByDescending(st => st.Rows).ToList();
        }

        public void ResetCounters()
        {
            TotalCalculatedSpins = 0;
            CurrentOverallSpins = 0;
        }

        public void SetTotalCalculatedSpins(int calcSpins) => TotalCalculatedSpins = calcSpins;

        public int GetTotalCalculatedSpins() => TotalCalculatedSpins;

        public void AddOverallSpin() => ++CurrentOverallSpins;

        public void AddDoneSpins(int remainingCount) => CurrentOverallSpins += remainingCount;

        public int GetCurrentOverallSpins() => CurrentOverallSpins;

        public double GetCurrentPercentage() => Math.Round(100.0 * CurrentOverallSpins / TotalCalculatedSpins, 2);
    }
}

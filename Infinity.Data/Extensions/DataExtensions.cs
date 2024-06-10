// Decompiled with JetBrains decompiler
// Type: Infinity.Data.Extensions.DataExtensions
// Assembly: Infinity.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A2C88E43-EACF-400C-AB56-5967E8E08F14
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Data.dll

using Infinity.Data.Models;
using Infinity.Engine;
using System;
using System.Linq;


#nullable enable
namespace Infinity.Data.Extensions
{
    public static class DataExtensions
    {
        public static bool HasRowLimit(this GameSetting gameSetting) => gameSetting.RowLimit.HasValue;

        public static bool HasCountLimit(this GameSetting gameSetting) => gameSetting.CountLimit.HasValue;

        public static bool HasGSLimit(this GameSetting gameSetting) => gameSetting.GSLimit.HasValue;

        public static bool HasR1WLimit(this GameSetting gameSetting) => gameSetting.R1WLimit.HasValue;

        public static bool HasTWLimit(this GameSetting gameSetting) => gameSetting.TWLimit.HasValue;

        public static double SpinPercentage(this GameSetting gameSetting)
        {
            lock (gameSetting)
                return Math.Round(100.0 * gameSetting.CurrentSpin / gameSetting.CalculatedSpinTotal, 2);
        }

        public static bool IsCount(this BoardNumber number) => number.Codes.Count() <= 1;
    }
}

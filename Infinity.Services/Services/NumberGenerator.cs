// Decompiled with JetBrains decompiler
// Type: Infinity.Services.Services.NumberGenerator
// Assembly: Infinity.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 168F0580-84D7-4BB1-A945-8997973C0544
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Services.dll

using Infinity.Services.Interfaces;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Infinity.Services.Services
{
  public class NumberGenerator : INumberGenerator
  {
    private Random _rand { get; set; }

    public NumberGenerator() => _rand = new Random();

    public async Task<int> NextRandomNumberAsync() => await Task.Run(() =>
    {
        lock (_rand)
            return _rand.Next(1, 37);
    });

    public int NextRandomNumber()
    {
      lock (_rand)
        return _rand.Next(1, 37);
    }
  }
}

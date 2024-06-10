// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.Counter
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll


#nullable enable
namespace Infinity.Engine
{
  public class Counter
  {
    private int _numberValue { get; set; }

    private int _count { get; set; }

    public Counter(int numberValue, int count)
    {
      _numberValue = numberValue;
      _count = count;
    }

    public int NumberValue
    {
      get => _numberValue;
      set => _numberValue = value;
    }

    public int Count
    {
      get => _count;
      set => _count = value;
    }

    public int CompareTo(Counter other) => other.Count.CompareTo(Count);
  }
}

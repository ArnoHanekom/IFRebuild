// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.Permutations
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System.Collections.Generic;


#nullable enable
namespace Infinity.Engine
{
  public class Permutations
  {
    private static List<List<int>> _currentList { get; set; } = new List<List<int>>(720);

    public static List<List<int>> CurrentList => Permutations._currentList;

    public static void Permut(int k, int n, int[] nums)
    {
      lock (Permutations._currentList)
        Permutations._currentList = new PermutationOptions(k, n, nums).ItemList;
    }
  }
}

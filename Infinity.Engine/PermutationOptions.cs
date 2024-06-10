// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.PermutationOptions
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System.Collections.Generic;


#nullable enable
namespace Infinity.Engine
{
  public class PermutationOptions
  {
    private List<List<int>> _itemList { get; set; } = new List<List<int>>(720);

    public List<List<int>> ItemList => _itemList;

    public PermutationOptions(int k, int n, int[] nums) => Permut(k, n, nums);

    private void Permut(int k, int n, int[] nums)
    {
      if (k <= n)
      {
        for (int index1 = k; index1 <= n; ++index1)
        {
          int num = nums[index1];
          for (int index2 = index1; index2 > k; --index2)
            nums[index2] = nums[index2 - 1];
          nums[k] = num;
          Permut(k + 1, n, nums);
          for (int index3 = k; index3 < index1; ++index3)
            nums[index3] = nums[index3 + 1];
          nums[index1] = num;
        }
      }
      else
        _itemList.AddPermut(nums);
    }
  }
}

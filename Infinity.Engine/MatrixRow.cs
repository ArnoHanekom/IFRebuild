// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.MatrixRow
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System.Collections.Generic;


#nullable enable
namespace Infinity.Engine
{
  public class MatrixRow
  {
    private List<MatrixNumber> _numbers { get; set; } = new List<MatrixNumber>(6);

    public List<MatrixNumber> Numbers
    {
      get => _numbers;
      set => _numbers = value;
    }

    public override string ToString() => string.Join("\t", _numbers);
  }
}

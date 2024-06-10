// Decompiled with JetBrains decompiler
// Type: Infinity.Engine.SpinEventArgs
// Assembly: Infinity.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8AAEFEEA-051B-4395-AA02-B0DBC05BFF91
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Engine.dll

using System;

namespace Infinity.Engine
{
  public class SpinEventArgs : EventArgs
  {
    public int Number { get; set; }
        public Guid SpinId { get; set; }
        public Guid TableId { get; set; }
        public Guid GameId { get; set; }
        public int SpinType { get; set; }
        public int PhaseType { get; set; }
  }
}

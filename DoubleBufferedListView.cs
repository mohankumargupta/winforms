// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.DoubleBufferedListView
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  public class DoubleBufferedListView : ListView
  {
    public DoubleBufferedListView()
    {
      this.DoubleBuffered = true;
    }
  }
}

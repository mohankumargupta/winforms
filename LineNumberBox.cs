// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.LineNumberBox
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  internal class LineNumberBox : Control
  {
    private int privateTopY;

    public int topY
    {
      get
      {
        return this.privateTopY;
      }
      set
      {
        int num = this.privateTopY;
        this.privateTopY = value;
        if (value == num)
          return;
        this.Invalidate();
      }
    }

    public LineNumberBox()
    {
      this.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      int num = 1;
      Graphics graphics = this.CreateGraphics();
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      Brush brush = (Brush) new SolidBrush(this.ForeColor);
      Point point = new Point(1, this.topY);
      while (point.Y <= this.Height)
      {
        graphics.DrawString(num.ToString().PadLeft(4), this.Font, brush, (PointF) point);
        point.Y += this.Font.Height;
        ++num;
      }
    }
  }
}

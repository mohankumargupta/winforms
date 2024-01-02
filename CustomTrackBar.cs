// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.CustomTrackBar
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Pololu.MaestroControlCenter
{
  internal class CustomTrackBar : Control
  {
    private int numberTicks = 5;
    private Rectangle trackRectangle = new Rectangle();
    private Rectangle ticksRectangle = new Rectangle();
    private Rectangle thumbRectangle = new Rectangle();
    private TrackBarThumbState thumbState = TrackBarThumbState.Normal;
    private Decimal privateIncrement = new Decimal(10);
    private Decimal privateMaximum = new Decimal(100);
    private Decimal privateValue = new Decimal(50);
    private bool privateShadowVisible = true;
    private Brush ellipseBrush = (Brush)new SolidBrush(Color.FromArgb(200, 0, 0, (int)byte.MaxValue));
    private Brush ellipseBrushEqual = (Brush)new SolidBrush(Color.FromArgb(200, 0, 200, 0));
    private Pen transparentDarkGrayPen = new Pen((Brush)new SolidBrush(Color.FromArgb(64, Color.DarkGray)));
    private int currentTickPosition;
    private float tickSpace;
    private bool thumbClicked;
    private Decimal privateMinimum;
    public Decimal privateShadowValue;
    public EventHandler ValueChanged;
    private int realThumbWidth;
    private int clickStartLocation;
    private int thumbStartLocation;

    public Decimal Increment
    {
      get
      {
        return this.privateIncrement;
      }
      set
      {
        this.privateIncrement = value;
      }
    }

    public Decimal Minimum
    {
      get
      {
        return this.privateMinimum;
      }
      set
      {
        if (!(this.privateMinimum != value))
          return;
        this.privateMinimum = value;
        this.thumbRectangle.X = this.computeThumbXFromValue(this.privateValue);
        this.Invalidate();
      }
    }

    public Decimal Maximum
    {
      get
      {
        return this.privateMaximum;
      }
      set
      {
        if (!(this.privateMaximum != value))
          return;
        this.privateMaximum = value;
        this.thumbRectangle.X = this.computeThumbXFromValue(this.privateValue);
        this.Invalidate();
      }
    }

    public Decimal Value
    {
      get
      {
        return this.privateValue;
      }
      set
      {
        if (value < this.Minimum)
          value = this.Minimum;
        if (value > this.Maximum)
          value = this.Maximum;
        if (!(value != this.privateValue))
          return;
        this.privateValue = value;
        if (this.ValueChanged != null)
          this.ValueChanged((object)this, (EventArgs)null);
        this.thumbRectangle.X = this.computeThumbXFromValue(value);
        this.Invalidate();
      }
    }

    public Decimal ShadowValue
    {
      get
      {
        return this.privateShadowValue;
      }
      set
      {
        if (value < this.Minimum)
          value = this.Minimum;
        if (value > this.Maximum)
          value = this.Maximum;
        if (!(value != this.privateShadowValue))
          return;
        this.privateShadowValue = value;
        this.Invalidate();
      }
    }

    public bool ShadowVisible
    {
      get
      {
        return this.privateShadowVisible;
      }
      set
      {
        if (value == this.privateShadowVisible)
          return;
        this.privateShadowVisible = value;
        this.Invalidate();
      }
    }

    public CustomTrackBar()
    {
      this.DoubleBuffered = true;
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    }

    public void HideThumb()
    {
      this.privateValue = new Decimal(50);
      this.thumbRectangle.X = -200;
      this.Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
      Graphics graphics = this.CreateGraphics();
      if (!TrackBarRenderer.IsSupported)
      {
        this.thumbRectangle.Size = new Size(9, 14);
        this.realThumbWidth = this.thumbRectangle.Width;
      }
      else
      {
        this.thumbRectangle.Size = TrackBarRenderer.GetBottomPointingThumbSize(graphics, TrackBarThumbState.Normal);
        this.realThumbWidth = this.thumbRectangle.Width - 2;
      }
      int num = this.thumbRectangle.Width / 2;
      this.trackRectangle.X = this.ClientRectangle.X + num;
      this.trackRectangle.Y = this.ClientRectangle.Y + 8;
      this.trackRectangle.Width = this.ClientRectangle.Width - num * 2;
      this.trackRectangle.Height = 4;
      this.ticksRectangle.X = this.trackRectangle.X;
      this.ticksRectangle.Y = this.ClientRectangle.Y + 16;
      this.ticksRectangle.Width = this.trackRectangle.Width;
      this.ticksRectangle.Height = 4;
      this.tickSpace = (float)(((double)this.ticksRectangle.Width - 1.0) / ((double)this.numberTicks - 1.0));
      this.thumbRectangle.X = this.CurrentTickXCoordinate();
      this.thumbRectangle.Y = this.ClientRectangle.Y;
    }

    private int CurrentTickXCoordinate()
    {
      if ((double)this.tickSpace == 0.0)
        return 0;
      else
        return (int)Math.Round((double)this.tickSpace) * this.currentTickPosition;
    }

    /*
    protected int computeThumbXFromValue(Decimal value)
    {
      if (this.Maximum <= this.Minimum)
        return this.trackRectangle.X + 1 - this.thumbRectangle.Width / 2;
      else
        return (int)Math.Round((Decimal)(this.trackRectangle.X - this.realThumbWidth / 2) + (value - this.Minimum) / (this.Maximum - this.Minimum) * (Decimal)(this.trackRectangle.Width - 2));
    }
    */
    public Decimal computeValueFromThumbX(Decimal x)
    {
      //return x;
      return this.Minimum + ((x - (Decimal)this.trackRectangle.X - 1 + (Decimal)(this.thumbRectangle.Width / 2)) * (this.Maximum - this.Minimum)) / (Decimal)(this.trackRectangle.Width - 2);
    }

    public int computeThumbXFromValue(Decimal value)
    {
      //return (int)value;
      return this.Maximum <= this.Minimum ? this.trackRectangle.X + 1 - this.thumbRectangle.Width / 2 : (int)Math.Round((Decimal)(this.trackRectangle.X - this.realThumbWidth / 2) + (value - this.Minimum) / (this.Maximum - this.Minimum) * (Decimal)(this.trackRectangle.Width - 2));
    }


    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      int num1 = 0;
      while (num1 <= 100)
      {
        int num2 = this.ticksRectangle.X + (this.ticksRectangle.Width - 2) * num1 / 100;
        int y = this.ticksRectangle.Y;
        int y2 = this.ticksRectangle.Y + this.ticksRectangle.Width - 1;
        graphics.DrawLine(Pens.DarkGray, num2, y, num2, y2);
        graphics.DrawLine(Pens.DarkGray, num2 + 1, y, num2 + 1, y2);
        num1 += 25;
      }
      if (!TrackBarRenderer.IsSupported)
      {
        graphics.DrawLine(Pens.DarkGray, this.trackRectangle.X, this.trackRectangle.Y, this.trackRectangle.X + this.trackRectangle.Width - 1, this.trackRectangle.Y);
        graphics.DrawLine(Pens.White, this.trackRectangle.X, this.trackRectangle.Y + 1, this.trackRectangle.X + this.trackRectangle.Width - 1, this.trackRectangle.Y + 1);
        GraphicsPath graphicsPath1 = new GraphicsPath();
        GraphicsPath graphicsPath2 = new GraphicsPath();
        GraphicsPath path = new GraphicsPath();
        int num2 = this.realThumbWidth;
        graphicsPath1.AddLine(this.thumbRectangle.X + num2, this.thumbRectangle.Y, this.thumbRectangle.X + num2, this.thumbRectangle.Y + this.thumbRectangle.Height - num2 / 2);
        graphicsPath1.AddLine(this.thumbRectangle.X + num2, this.thumbRectangle.Y + this.thumbRectangle.Height - num2 / 2, this.thumbRectangle.X + num2 / 2 + 1, this.thumbRectangle.Y + this.thumbRectangle.Height);
        graphicsPath2.AddLine(this.thumbRectangle.X + num2 / 2, this.thumbRectangle.Y + this.thumbRectangle.Height, this.thumbRectangle.X, this.thumbRectangle.Y + this.thumbRectangle.Height - num2 / 2);
        graphicsPath2.AddLine(this.thumbRectangle.X, this.thumbRectangle.Y + this.thumbRectangle.Height - num2 / 2, this.thumbRectangle.X, this.thumbRectangle.Y);
        graphicsPath2.AddLine(this.thumbRectangle.X, this.thumbRectangle.Y, this.thumbRectangle.X + num2, this.thumbRectangle.Y);
        path.AddPath(graphicsPath1, true);
        path.AddPath(graphicsPath2, true);
        graphics.FillPath(Brushes.LightGray, path);
        graphics.DrawPath(Pens.DarkGray, graphicsPath1);
        graphics.DrawPath(this.transparentDarkGrayPen, graphicsPath2);
      }
      else
      {
        TrackBarRenderer.DrawHorizontalTrack(graphics, this.trackRectangle);
        TrackBarRenderer.DrawBottomPointingThumb(graphics, this.thumbRectangle, this.thumbState);
      }
      if (!this.ShadowVisible)
        return;
      Rectangle rect = new Rectangle(new Point(this.computeThumbXFromValue(this.ShadowValue) + 1, this.trackRectangle.Y - this.realThumbWidth / 2), new Size(this.realThumbWidth - 2, this.realThumbWidth - 2));
      if (this.Value == this.ShadowValue)
        graphics.FillEllipse(this.ellipseBrushEqual, rect);
      else
        graphics.FillEllipse(this.ellipseBrush, rect);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      if (this.thumbRectangle.Contains(e.Location))
      {
        this.clickStartLocation = e.X;
        this.thumbStartLocation = this.thumbRectangle.X;
        this.thumbClicked = true;
        this.thumbState = TrackBarThumbState.Pressed;
      }
      else if (e.X < this.thumbRectangle.X)
        this.Value -= this.Increment;
      else
        this.Value += this.Increment;
      this.Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      if (!this.thumbClicked)
        return;
      if (e.Location.X > this.trackRectangle.X && e.Location.X < this.trackRectangle.X + this.trackRectangle.Width - this.thumbRectangle.Width)
      {
        this.thumbClicked = false;
        this.thumbState = TrackBarThumbState.Hot;
        this.Invalidate();
      }
      this.thumbClicked = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        this.OnMouseUp(e);
      if (this.thumbClicked)
        this.Value = this.computeValueFromThumbX((Decimal)(this.thumbStartLocation + e.X - this.clickStartLocation));
      else
        this.thumbState = this.thumbRectangle.Contains(e.Location) ? TrackBarThumbState.Hot : TrackBarThumbState.Normal;
      this.Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      if (this.thumbState == TrackBarThumbState.Normal)
        return;
      this.thumbState = TrackBarThumbState.Normal;
      this.Invalidate();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.RoundingNumericUpDown
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System;
using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  internal class RoundingNumericUpDown : NumericUpDown
  {
    private Decimal privateMinimumIncrement = new Decimal(1);
    private int privateMantissaBits;
    private bool supressEvents;

    public int MantissaBits
    {
      get
      {
        return this.privateMantissaBits;
      }
      set
      {
        this.privateMantissaBits = value;
        this.updateIncrement();
      }
    }

    private byte Exponent
    {
      get
      {
        uint num1 = (uint) (this.Value / this.MinimumIncrement);
        byte num2 = (byte) 0;
        while ((long) num1 >= (long) (1 << this.MantissaBits))
        {
          ++num2;
          num1 >>= 1;
        }
        return num2;
      }
    }

    public Decimal MinimumIncrement
    {
      get
      {
        return this.privateMinimumIncrement;
      }
      set
      {
        this.privateMinimumIncrement = value;
        this.updateIncrement();
      }
    }

    public RoundingNumericUpDown()
    {
      this.Leave += new EventHandler(this.RoundingNumericUpDown_ValueChanged);
      this.ValueChanged += new EventHandler(this.RoundingNumericUpDown_ValueChanged);
    }

    private void updateIncrement()
    {
      if (this.MantissaBits == 0 || !(this.Increment != (Decimal) (1 << (int) this.Exponent) * this.MinimumIncrement))
        return;
      this.Increment = (Decimal) (1 << (int) this.Exponent) * this.MinimumIncrement;
    }

    private void RoundingNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      if (this.supressEvents)
        return;
      this.supressEvents = true;
      this.updateIncrement();
      this.round();
      this.supressEvents = false;
    }

    private void round()
    {
      Math.Max(this.Minimum, Math.Min(this.Maximum, this.Value));
      Decimal num = Math.Round(this.Value / this.Increment, MidpointRounding.AwayFromZero) * this.Increment;
      if (num > this.Maximum)
        num -= this.Increment;
      if (num < this.Minimum)
        num += this.Increment;
      this.Value = num;
    }

    public void safeSetValue(Decimal newValue)
    {
      if (newValue > this.Maximum)
        this.Value = this.Maximum;
      else if (newValue < this.Minimum)
        this.Value = this.Minimum;
      else
        this.Value = newValue;
    }
  }
}

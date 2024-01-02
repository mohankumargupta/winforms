// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.ServoStatusControl
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using Pololu.Usc;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  public class ServoStatusControl : UserControl
  {
    public ServoStatusControl.UshortDelegate targetCallback;
    public ServoStatusControl.UshortDelegate speedCallback;
    public ServoStatusControl.UshortDelegate accelerationCallback;
    public bool supressEvents;
    private ChannelMode privateMode;
    public ushort home;
    private IContainer components;
    private Label servoNumberLabel;
    private RoundingNumericUpDown positionUpDown;
    private NumericUpDown accelerationUpDown;
    private NumericUpDown speedUpDown;
    private NumericUpDown targetUpDown;
    private CheckBox onOffCheckBox;
    private Label targetOutOfRangeLabel;
    private CustomTrackBar customTrackBar1;
    private Label servoNameLabel;
    private ToolTip toolTip1;
    private Label modeLabel;

    public ChannelMode mode
    {
      set
      {
        this.privateMode = value;
        this.speedUpDown.ReadOnly = this.accelerationUpDown.ReadOnly = value != ChannelMode.Servo && value != ChannelMode.ServoMultiplied;
        this.onOffCheckBox.Enabled = value != ChannelMode.Input;
        this.targetUpDown.ReadOnly = value == ChannelMode.Input && (int) this.target == 0;
        switch (value)
        {
          case ChannelMode.Servo:
            this.modeLabel.Text = "Servo";
            break;
          case ChannelMode.ServoMultiplied:
            this.modeLabel.Text = "Servo";
            break;
          case ChannelMode.Output:
            this.modeLabel.Text = "Output";
            break;
          case ChannelMode.Input:
            this.modeLabel.Text = "Input";
            break;
        }
      }
    }

    public int servoNumber
    {
      get
      {
        return int.Parse(this.servoNumberLabel.Text);
      }
      set
      {
        this.servoNumberLabel.Text = value.ToString();
      }
    }

    public string servoName
    {
      get
      {
        return this.servoNameLabel.Text;
      }
      set
      {
        this.servoNameLabel.Text = value;
      }
    }

    public ushort position
    {
      get
      {
        if (!this.onOffCheckBox.Checked)
          return (ushort) 0;
        else
          return (ushort) (new Decimal(4) * this.positionUpDown.Value);
      }
      set
      {
        this.positionUpDown.Value = (Decimal) value / new Decimal(4);
        this.supressEvents = true;
        if ((int) value == 0 && (int) this.min != 0)
        {
          this.customTrackBar1.ShadowVisible = false;
        }
        else
        {
          this.customTrackBar1.ShadowVisible = true;
          this.customTrackBar1.ShadowValue = this.positionUpDown.Value;
        }
        this.supressEvents = false;
      }
    }

    public ushort target
    {
      get
      {
        if (!this.onOffCheckBox.Checked)
          return (ushort) 0;
        else
          return (ushort) (new Decimal(4) * this.targetUpDown.Value);
      }
      set
      {
        this.supressEvents = true;
        if ((int) value == 0 && (int) this.min != 0)
        {
          this.onOffCheckBox.Checked = false;
          this.disableTarget();
        }
        else
        {
          if ((int) value > (int) this.max)
          {
            value = this.max;
            this.targetOutOfRangeLabel.Visible = true;
          }
          else if ((int) value < (int) this.min)
          {
            value = this.min;
            this.targetOutOfRangeLabel.Visible = true;
          }
          else
            this.targetOutOfRangeLabel.Visible = false;
          this.onOffCheckBox.Checked = true;
          this.enableTarget();
          this.targetUpDown.Value = this.customTrackBar1.Value = (Decimal) value / new Decimal(4);
        }
        this.supressEvents = false;
      }
    }

    public ushort speed
    {
      get
      {
        return (ushort) this.speedUpDown.Value;
      }
      set
      {
        this.supressEvents = true;
        this.speedUpDown.Value = (Decimal) value;
        this.supressEvents = false;
      }
    }

    public ushort acceleration
    {
      get
      {
        return (ushort) this.accelerationUpDown.Value;
      }
      set
      {
        this.supressEvents = true;
        this.accelerationUpDown.Value = (Decimal) value;
        this.supressEvents = false;
      }
    }

    public ushort min
    {
      get
      {
        return (ushort) (this.targetUpDown.Minimum * new Decimal(4));
      }
      set
      {
        this.customTrackBar1.Minimum = (Decimal) value / new Decimal(4);
        this.targetUpDown.Minimum = (Decimal) value / new Decimal(4);
      }
    }

    public ushort max
    {
      get
      {
        return (ushort) (this.targetUpDown.Maximum * new Decimal(4));
      }
      set
      {
        this.customTrackBar1.Maximum = (Decimal) value / new Decimal(4);
        this.targetUpDown.Maximum = (Decimal) value / new Decimal(4);
      }
    }

    public ServoStatusControl()
    {
      this.InitializeComponent();
      this.customTrackBar1.ValueChanged = new EventHandler(this.targetTrackBar_Scroll);
    }

    public void resetTargetState()
    {
      this.supressEvents = true;
      this.targetUpDown.Value = !(this.targetUpDown.Minimum == new Decimal(992)) || !(this.targetUpDown.Maximum == new Decimal(2000)) ? (this.targetUpDown.Minimum + this.targetUpDown.Maximum) / new Decimal(2) : new Decimal(1500);
      this.customTrackBar1.HideThumb();
      this.supressEvents = false;
    }

    private void targetTrackBar_Scroll(object sender, EventArgs e)
    {
      if (this.supressEvents)
        return;
      if (!this.accelerationUpDown.ContainsFocus)
      {
        if (!this.speedUpDown.ContainsFocus)
          goto label_4;
      }
      this.customTrackBar1.Focus();
label_4:
      try
      {
        this.targetUpDown.Value = this.customTrackBar1.Value;
      }
      catch (ArgumentException ex)
      {
      }
    }

    private void targetUpDown_ValueChanged(object sender, EventArgs e)
    {
      this.customTrackBar1.Value = this.targetUpDown.Value;
      if (this.targetCallback == null || this.supressEvents)
        return;
      this.targetCallback((ushort) (this.targetUpDown.Value * new Decimal(4)));
    }

    private void speedUpDown_ValueChanged(object sender, EventArgs e)
    {
      if (this.speedCallback == null || this.supressEvents)
        return;
      this.speedCallback((ushort) this.speedUpDown.Value);
    }

    private void accelerationUpDown_ValueChanged(object sender, EventArgs e)
    {
      if (this.accelerationCallback == null || this.supressEvents)
        return;
      this.accelerationCallback((ushort) (byte) this.accelerationUpDown.Value);
    }

    private void disableTarget()
    {
      this.customTrackBar1.Enabled = false;
      this.targetUpDown.ReadOnly = true;
    }

    private void enableTarget()
    {
      this.customTrackBar1.Enabled = true;
      this.targetUpDown.ReadOnly = this.privateMode == ChannelMode.Input;
    }

    private void onOffCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (this.onOffCheckBox.Checked)
      {
        this.enableTarget();
        this.targetUpDown_ValueChanged((object) null, (EventArgs) null);
      }
      else
      {
        this.disableTarget();
        if (this.targetCallback == null || this.supressEvents)
          return;
        this.targetCallback((ushort) 0);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ServoStatusControl));
      this.servoNumberLabel = new Label();
      this.speedUpDown = new NumericUpDown();
      this.targetUpDown = new NumericUpDown();
      this.onOffCheckBox = new CheckBox();
      this.targetOutOfRangeLabel = new Label();
      this.servoNameLabel = new Label();
      this.accelerationUpDown = new NumericUpDown();
      this.toolTip1 = new ToolTip(this.components);
      this.customTrackBar1 = new CustomTrackBar();
      this.positionUpDown = new RoundingNumericUpDown();
      this.modeLabel = new Label();
      this.speedUpDown.BeginInit();
      this.targetUpDown.BeginInit();
      this.accelerationUpDown.BeginInit();
      this.positionUpDown.BeginInit();
      this.SuspendLayout();
      this.servoNumberLabel.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);
      this.servoNumberLabel.Location = new Point(0, 1);
      this.servoNumberLabel.Name = "servoNumberLabel";
      this.servoNumberLabel.Padding = new Padding(0, 0, 1, 0);
      this.servoNumberLabel.Size = new Size(27, 20);
      this.servoNumberLabel.TabIndex = 0;
      this.servoNumberLabel.Text = "0";
      this.servoNumberLabel.TextAlign = ContentAlignment.TopRight;
      this.speedUpDown.Location = new Point(496, 1);
      NumericUpDown numericUpDown1 = this.speedUpDown;
      int[] bits1 = new int[4];
      bits1[0] = 10000;
      Decimal num1 = new Decimal(bits1);
      numericUpDown1.Maximum = num1;
      this.speedUpDown.Name = "speedUpDown";
      this.speedUpDown.Size = new Size(70, 20);
      this.speedUpDown.TabIndex = 7;
      this.speedUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control) this.speedUpDown, "Sets and displays the current maximum speed of the servo.\r\nA value of 0 corresponds to unlimited speed.\r\nThis setting has no effect for an input or output channel.\r\n");
      this.speedUpDown.ValueChanged += new EventHandler(this.speedUpDown_ValueChanged);
      this.targetUpDown.DecimalPlaces = 2;
      this.targetUpDown.Increment = new Decimal(new int[4]
      {
        25,
        0,
        0,
        131072
      });
      this.targetUpDown.Location = new Point(420, 1);
      NumericUpDown numericUpDown2 = this.targetUpDown;
      int[] bits2 = new int[4];
      bits2[0] = 2500;
      Decimal num2 = new Decimal(bits2);
      numericUpDown2.Maximum = num2;
      NumericUpDown numericUpDown3 = this.targetUpDown;
      int[] bits3 = new int[4];
      bits3[0] = 50;
      Decimal num3 = new Decimal(bits3);
      numericUpDown3.Minimum = num3;
      this.targetUpDown.Name = "targetUpDown";
      this.targetUpDown.Size = new Size(70, 20);
      this.targetUpDown.TabIndex = 6;
      this.targetUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control) this.targetUpDown, "Enter a numerical value for the Target\r\n(in μs) for precise control of a servo or\r\noutput channel.  For input channels, this\r\nwill display the current input value, from 0\r\nto 255.75 (0 to 5 V).");
      NumericUpDown numericUpDown4 = this.targetUpDown;
      int[] bits4 = new int[4];
      bits4[0] = 1500;
      Decimal num4 = new Decimal(bits4);
      numericUpDown4.Value = num4;
      this.targetUpDown.ValueChanged += new EventHandler(this.targetUpDown_ValueChanged);
      this.onOffCheckBox.AutoSize = true;
      this.onOffCheckBox.Location = new Point(174, 4);
      this.onOffCheckBox.Name = "onOffCheckBox";
      this.onOffCheckBox.Size = new Size(15, 14);
      this.onOffCheckBox.TabIndex = 3;
      this.toolTip1.SetToolTip((Control) this.onOffCheckBox, componentResourceManager.GetString("onOffCheckBox.ToolTip"));
      this.onOffCheckBox.UseVisualStyleBackColor = true;
      this.onOffCheckBox.CheckedChanged += new EventHandler(this.onOffCheckBox_CheckedChanged);
      this.targetOutOfRangeLabel.AutoSize = true;
      this.targetOutOfRangeLabel.BackColor = Color.Transparent;
      this.targetOutOfRangeLabel.ForeColor = Color.Red;
      this.targetOutOfRangeLabel.Location = new Point(252, 3);
      this.targetOutOfRangeLabel.Name = "targetOutOfRangeLabel";
      this.targetOutOfRangeLabel.Size = new Size(98, 13);
      this.targetOutOfRangeLabel.TabIndex = 5;
      this.targetOutOfRangeLabel.Text = "Target out of range";
      this.servoNameLabel.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.servoNameLabel.Location = new Point(31, 4);
      this.servoNameLabel.Name = "servoNameLabel";
      this.servoNameLabel.Size = new Size(83, 17);
      this.servoNameLabel.TabIndex = 1;
      this.accelerationUpDown.Location = new Point(572, 1);
      NumericUpDown numericUpDown5 = this.accelerationUpDown;
      int[] bits5 = new int[4];
      bits5[0] = (int) byte.MaxValue;
      Decimal num5 = new Decimal(bits5);
      numericUpDown5.Maximum = num5;
      this.accelerationUpDown.Name = "accelerationUpDown";
      this.accelerationUpDown.Size = new Size(70, 20);
      this.accelerationUpDown.TabIndex = 8;
      this.accelerationUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control) this.accelerationUpDown, "Sets and displays the acceleration limit of the servo.\r\nA value of 0 corresponds to unlimited acceleration.\r\nThis setting has no effect for an input or output channel.\r\n");
      this.accelerationUpDown.ValueChanged += new EventHandler(this.accelerationUpDown_ValueChanged);
      CustomTrackBar customTrackBar1 = this.customTrackBar1;
      int[] bits6 = new int[4];
      bits6[0] = 10;
      Decimal num6 = new Decimal(bits6);
      customTrackBar1.Increment = num6;
      this.customTrackBar1.Location = new Point(196, 1);
      CustomTrackBar customTrackBar2 = this.customTrackBar1;
      int[] bits7 = new int[4];
      bits7[0] = 100;
      Decimal num7 = new Decimal(bits7);
      customTrackBar2.Maximum = num7;
      this.customTrackBar1.Minimum = new Decimal(new int[4]);
      this.customTrackBar1.Name = "customTrackBar1";
      this.customTrackBar1.ShadowValue = new Decimal(new int[4]);
      this.customTrackBar1.ShadowVisible = false;
      this.customTrackBar1.Size = new Size(218, 20);
      this.customTrackBar1.TabIndex = 4;
      this.customTrackBar1.Text = "customTrackBar1";
      this.toolTip1.SetToolTip((Control) this.customTrackBar1, componentResourceManager.GetString("customTrackBar1.ToolTip"));
      CustomTrackBar customTrackBar3 = this.customTrackBar1;
      int[] bits8 = new int[4];
      bits8[0] = 50;
      Decimal num8 = new Decimal(bits8);
      customTrackBar3.Value = num8;
      this.positionUpDown.DecimalPlaces = 2;
      this.positionUpDown.Increment = new Decimal(new int[4]
      {
        25,
        0,
        0,
        131072
      });
      this.positionUpDown.Location = new Point(648, 1);
      this.positionUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown1 = this.positionUpDown;
      int[] bits9 = new int[4];
      bits9[0] = 60000;
      Decimal num9 = new Decimal(bits9);
      roundingNumericUpDown1.Maximum = num9;
      RoundingNumericUpDown roundingNumericUpDown2 = this.positionUpDown;
      int[] bits10 = new int[4];
      bits10[0] = 1;
      Decimal num10 = new Decimal(bits10);
      roundingNumericUpDown2.MinimumIncrement = num10;
      this.positionUpDown.Name = "positionUpDown";
      this.positionUpDown.ReadOnly = true;
      this.positionUpDown.Size = new Size(70, 20);
      this.positionUpDown.TabIndex = 9;
      this.positionUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control) this.positionUpDown, componentResourceManager.GetString("positionUpDown.ToolTip"));
      RoundingNumericUpDown roundingNumericUpDown3 = this.positionUpDown;
      int[] bits11 = new int[4];
      bits11[0] = 1500;
      Decimal num11 = new Decimal(bits11);
      roundingNumericUpDown3.Value = num11;
      this.modeLabel.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.modeLabel.Location = new Point(117, 4);
      this.modeLabel.Name = "modeLabel";
      this.modeLabel.Size = new Size(57, 15);
      this.modeLabel.TabIndex = 2;
      this.modeLabel.Text = "Servo";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Transparent;
      this.Controls.Add((Control) this.modeLabel);
      this.Controls.Add((Control) this.servoNameLabel);
      this.Controls.Add((Control) this.customTrackBar1);
      this.Controls.Add((Control) this.targetOutOfRangeLabel);
      this.Controls.Add((Control) this.onOffCheckBox);
      this.Controls.Add((Control) this.targetUpDown);
      this.Controls.Add((Control) this.speedUpDown);
      this.Controls.Add((Control) this.positionUpDown);
      this.Controls.Add((Control) this.accelerationUpDown);
      this.Controls.Add((Control) this.servoNumberLabel);
      this.Margin = new Padding(0, 2, 0, 2);
      this.Name = "ServoStatusControl";
      this.Size = new Size(726, 21);
      this.speedUpDown.EndInit();
      this.targetUpDown.EndInit();
      this.accelerationUpDown.EndInit();
      this.positionUpDown.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public delegate void UshortDelegate(ushort arg);
  }
}

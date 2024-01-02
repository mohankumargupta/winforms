// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.ServoControl
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using Pololu.Usc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using winforms;

namespace Pololu.MaestroControlCenter
{
  public class ServoControl : UserControl
  {
    private ushort privateMaxLimit = (ushort)12000;
    public EventHandler changeStartCallback;
    private bool suppressChangeEvent;
    private bool privateOnlyAllowIO;
    private bool currentHomeValueValid;
    private IContainer components;
    private Label servoNumberLabel;
    private RoundingNumericUpDown homeUpDown;
    private RoundingNumericUpDown speedUpDown;
    private ComboBox modeComboBox;
    private ComboBox homeComboBox;
    private ToolTip toolTip1;
    private RoundingNumericUpDown accelerationUpDown;
    private RoundingNumericUpDown minUpDown;
    private RoundingNumericUpDown maxUpDown;
    private RoundingNumericUpDown rangeUpDown;
    private RoundingNumericUpDown neutralUpDown;
    private TextBox servoNameTextBox;
    private ComboBox pulseRateComboBox;
    private Label pulseRateLabel;

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
        return this.servoNameTextBox.Text;
      }
      set
      {
        this.servoNameTextBox.Text = value;
      }
    }

    public bool onlyAllowIO
    {
      get
      {
        return this.privateOnlyAllowIO;
      }
      set
      {
        this.privateOnlyAllowIO = value;
        if (!value || this.mode != ChannelMode.Servo && this.mode != ChannelMode.ServoMultiplied)
          return;
        this.mode = ChannelMode.Input;
      }
    }

    public ushort range
    {
      get
      {
        return (ushort)(this.rangeUpDown.Value * new Decimal(4));
      }
      set
      {
        this.rangeUpDown.safeSetValue((Decimal)value / new Decimal(4));
      }
    }

    public ushort neutral
    {
      get
      {
        return (ushort)(this.neutralUpDown.Value * new Decimal(4));
      }
      set
      {
        this.neutralUpDown.safeSetValue((Decimal)value / new Decimal(4));
      }
    }

    public ushort min
    {
      get
      {
        return (ushort)(this.minUpDown.Value * new Decimal(4));
      }
      set
      {
        this.minUpDown.safeSetValue((Decimal)value / new Decimal(4));
      }
    }

    public ushort max
    {
      get
      {
        return (ushort)(this.maxUpDown.Value * new Decimal(4));
      }
      set
      {
        this.maxUpDown.safeSetValue((Decimal)value / new Decimal(4));
      }
    }

    public ushort home
    {
      get
      {
        return (ushort)(this.homeUpDown.Value * new Decimal(4));
      }
      set
      {
        this.homeUpDown.safeSetValue((Decimal)value / new Decimal(4));
        this.currentHomeValueValid = false;
      }
    }

    public ushort maxLimit
    {
      get
      {
        return (ushort)((uint)this.privateMaxLimit * 4U);
      }
      set
      {
        if (this.mode == ChannelMode.Servo || this.mode == ChannelMode.ServoMultiplied)
          this.maxUpDown.Maximum = (Decimal)value / new Decimal(4);
        this.privateMaxLimit = value;
      }
    }

    public ushort speed
    {
      get
      {
        return (ushort)this.speedUpDown.Value;
      }
      set
      {
        this.speedUpDown.safeSetValue((Decimal)value);
      }
    }

    public byte acceleration
    {
      get
      {
        return (byte)this.accelerationUpDown.Value;
      }
      set
      {
        this.accelerationUpDown.safeSetValue((Decimal)value);
      }
    }

    public ChannelMode mode
    {
      get
      {
        switch (this.modeComboBox.SelectedIndex)
        {
          case 1:
            return ChannelMode.Input;
          case 2:
            return ChannelMode.Output;
          default:
            return this.pulseRateComboBox.SelectedIndex != 1 ? ChannelMode.Servo : ChannelMode.ServoMultiplied;
        }
      }
      set
      {
        switch (value)
        {
          case ChannelMode.ServoMultiplied:
            this.modeComboBox.SelectedIndex = 0;
            if (this.pulseRateComboBox.Items.Count < 2)
              this.pulseRateComboBox.SelectedIndex = 0;
            else
              this.pulseRateComboBox.SelectedIndex = 1;
            this.servoMode();
            break;
          case ChannelMode.Output:
            this.modeComboBox.SelectedIndex = 2;
            this.outputMode();
            break;
          case ChannelMode.Input:
            this.modeComboBox.SelectedIndex = 1;
            this.inputMode();
            break;
          default:
            this.modeComboBox.SelectedIndex = 0;
            this.pulseRateComboBox.SelectedIndex = 0;
            this.servoMode();
            break;
        }
      }
    }

    public HomeMode homeMode
    {
      get
      {
        switch (this.homeComboBox.SelectedIndex)
        {
          case 1:
            return HomeMode.Ignore;
          case 2:
            return HomeMode.Goto;
          default:
            return HomeMode.Off;
        }
      }
      set
      {
        switch (value)
        {
          case HomeMode.Ignore:
            this.homeComboBox.SelectedIndex = 1;
            break;
          case HomeMode.Goto:
            this.homeComboBox.SelectedIndex = 2;
            break;
          default:
            this.homeComboBox.SelectedIndex = 0;
            break;
        }
      }
    }

    public ServoControl()
    {
      this.InitializeComponent();
      this.modeComboBox.SelectedIndex = 0;
      this.homeComboBox.SelectedIndex = 0;
    }

    private void changeStart(object sender, KeyPressEventArgs e)
    {
      if (this.changeStartCallback == null)
        return;
      this.changeStartCallback(sender, (EventArgs)e);
    }

    private void change(object sender, EventArgs e)
    {
      if (this.suppressChangeEvent)
        return;
      if (sender == this.homeUpDown)
        this.currentHomeValueValid = true;
      this.suppressChangeEvent = true;
      this.neutralUpDown.Maximum = this.homeUpDown.Maximum = this.maxUpDown.Value;
      this.neutralUpDown.Minimum = this.homeUpDown.Minimum = this.minUpDown.Value;
      if (this.changeStartCallback != null)
        this.changeStartCallback(sender, e);
      this.suppressChangeEvent = false;
    }

    private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.mode == ChannelMode.Servo || this.mode == ChannelMode.ServoMultiplied)
      {
        if (this.onlyAllowIO)
        {
          //this.mode = ChannelMode.Input;
          // Form1.message("You must increase the number of servos available to use this port.", "Servo not available");
        }
        else
          this.servoMode();
      }
      if (this.modeComboBox.Text == "Input")
        this.inputMode();
      if (this.modeComboBox.Text == "Output")
        this.outputMode();
      this.enableHomeUpDownIfAllowed();
      this.change((object)null, (EventArgs)null);
    }

    private void servoMode()
    {
      this.minUpDown.Enabled = this.maxUpDown.Enabled = this.neutralUpDown.Enabled = this.speedUpDown.Enabled = this.accelerationUpDown.Enabled = this.rangeUpDown.Enabled = this.homeComboBox.Enabled = true;
      this.minUpDown.Minimum = new Decimal(50);
      this.minUpDown.safeSetValue(new Decimal(992));
      this.maxUpDown.Maximum = (Decimal)this.privateMaxLimit / new Decimal(4);
      this.maxUpDown.safeSetValue(new Decimal(2000));
      this.updatePulseControlVisibility();
    }

    private void outputMode()
    {
      this.minUpDown.Enabled = this.maxUpDown.Enabled = this.neutralUpDown.Enabled = this.speedUpDown.Enabled = this.accelerationUpDown.Enabled = this.neutralUpDown.Enabled = this.rangeUpDown.Enabled = false;
      this.homeComboBox.Enabled = true;
      this.maxUpDown.Maximum = new Decimal(2000);
      this.maxUpDown.Value = new Decimal(2000);
      this.minUpDown.Value = new Decimal(992);
      this.speedUpDown.Value = new Decimal(0);
      this.accelerationUpDown.Value = new Decimal(0);
      this.neutralUpDown.Value = new Decimal(1500);
      this.rangeUpDown.Value = new Decimal(47625, 0, 0, false, (byte)2);
      this.pulseRateComboBox.Visible = false;
      this.pulseRateLabel.Visible = false;
    }

    private void inputMode()
    {
      this.minUpDown.Enabled = this.maxUpDown.Enabled = this.neutralUpDown.Enabled = this.speedUpDown.Enabled = this.accelerationUpDown.Enabled = this.neutralUpDown.Enabled = this.rangeUpDown.Enabled = this.homeComboBox.Enabled = false;
      this.homeMode = HomeMode.Ignore;
      this.minUpDown.Minimum = new Decimal(0);
      this.minUpDown.Value = new Decimal(0);
      this.maxUpDown.Maximum = new Decimal(256);
      this.maxUpDown.Value = new Decimal(256);
      this.pulseRateComboBox.Visible = false;
      this.pulseRateLabel.Visible = false;
    }

    private void enableHomeUpDownIfAllowed()
    {
      if ((this.modeComboBox.Text == "Servo" || this.modeComboBox.Text == "Output") && this.homeComboBox.Text == "Go to")
      {
        this.homeUpDown.Enabled = true;
        if (!this.currentHomeValueValid)
        {
          this.homeUpDown.safeSetValue((this.minUpDown.Value + this.maxUpDown.Value) / new Decimal(2));
          this.currentHomeValueValid = false;
        }
        if (!this.ContainsFocus)
          return;
        this.homeUpDown.Select(0, 10);
        this.homeUpDown.Focus();
      }
      else
        this.homeUpDown.Enabled = false;
    }

    private void homeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.enableHomeUpDownIfAllowed();
      this.change((object)null, (EventArgs)null);
    }

    public void setPulseRateOptions(List<int> pulseRates)
    {
      if (pulseRates.Count == 0)
      {
        this.pulseRateComboBox.Visible = false;
        this.pulseRateLabel.Visible = false;
      }
      else
      {
        this.pulseRateLabel.Text = pulseRates[0].ToString();
        int selectedIndex = this.pulseRateComboBox.SelectedIndex;
        this.pulseRateComboBox.Items.Clear();
        foreach (int num in pulseRates)
          this.pulseRateComboBox.Items.Add((object)num.ToString());
        if (selectedIndex < pulseRates.Count && selectedIndex != -1)
          this.pulseRateComboBox.SelectedIndex = selectedIndex;
        else
          this.pulseRateComboBox.SelectedIndex = 0;
        this.updatePulseControlVisibility();
      }
    }

    private void updatePulseControlVisibility()
    {
      if (this.modeComboBox.SelectedIndex == 0)
      {
        if (this.pulseRateComboBox.Items.Count > 1)
        {
          this.pulseRateComboBox.Visible = true;
          this.pulseRateLabel.Visible = false;
        }
        else
        {
          this.pulseRateComboBox.Visible = false;
          this.pulseRateLabel.Visible = true;
        }
      }
      else
      {
        this.pulseRateComboBox.Visible = false;
        this.pulseRateLabel.Visible = false;
      }
    }

    public ChannelSetting getChannelSetting()
    {
      return new ChannelSetting()
      {
        name = this.servoName,
        mode = this.mode,
        maximum = this.max,
        minimum = this.min,
        homeMode = this.homeMode,
        home = this.home,
        speed = this.speed,
        acceleration = this.acceleration,
        neutral = this.neutral,
        range = this.range
      };
    }

    public void set(ChannelSetting setting)
    {
      this.servoName = setting.name;
      this.mode = setting.mode;
      this.max = setting.maximum;
      this.min = setting.minimum;
      this.homeMode = setting.homeMode;
      this.home = setting.home;
      this.currentHomeValueValid = this.homeMode == HomeMode.Goto;
      this.speed = setting.speed;
      this.acceleration = setting.acceleration;
      this.neutral = setting.neutral;
      this.range = setting.range;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer)new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ServoControl));
      this.servoNumberLabel = new Label();
      this.modeComboBox = new ComboBox();
      this.homeComboBox = new ComboBox();
      this.toolTip1 = new ToolTip(this.components);
      this.pulseRateComboBox = new ComboBox();
      this.servoNameTextBox = new TextBox();
      this.pulseRateLabel = new Label();
      this.speedUpDown = new RoundingNumericUpDown();
      this.rangeUpDown = new RoundingNumericUpDown();
      this.neutralUpDown = new RoundingNumericUpDown();
      this.homeUpDown = new RoundingNumericUpDown();
      this.maxUpDown = new RoundingNumericUpDown();
      this.minUpDown = new RoundingNumericUpDown();
      this.accelerationUpDown = new RoundingNumericUpDown();
      this.speedUpDown.BeginInit();
      this.rangeUpDown.BeginInit();
      this.neutralUpDown.BeginInit();
      this.homeUpDown.BeginInit();
      this.maxUpDown.BeginInit();
      this.minUpDown.BeginInit();
      this.accelerationUpDown.BeginInit();
      this.SuspendLayout();
      this.servoNumberLabel.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);
      this.servoNumberLabel.Location = new Point(0, 1);
      this.servoNumberLabel.Name = "servoNumberLabel";
      this.servoNumberLabel.Padding = new Padding(0, 0, 1, 0);
      this.servoNumberLabel.Size = new Size(27, 20);
      this.servoNumberLabel.TabIndex = 0;
      this.servoNumberLabel.Text = "0";
      this.servoNumberLabel.TextAlign = ContentAlignment.TopRight;
      this.modeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.modeComboBox.FormattingEnabled = true;
      this.modeComboBox.Items.AddRange(new object[3]
      {
        (object) "Servo",
        (object) "Input",
        (object) "Output"
      });
      this.modeComboBox.Location = new Point(120, 0);
      this.modeComboBox.Name = "modeComboBox";
      this.modeComboBox.Size = new Size(65, 21);
      this.modeComboBox.TabIndex = 1;
      this.toolTip1.SetToolTip((Control)this.modeComboBox, "Selects the mode of the port: servo, analog input, or digital output.");
      this.modeComboBox.SelectedIndexChanged += new EventHandler(this.modeComboBox_SelectedIndexChanged);
      this.homeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.homeComboBox.FormattingEnabled = true;
      this.homeComboBox.Items.AddRange(new object[3]
      {
        (object) "Off",
        (object) "Ignore",
        (object) "Go to"
      });
      this.homeComboBox.Location = new Point(374, 0);
      this.homeComboBox.Name = "homeComboBox";
      this.homeComboBox.Size = new Size(56, 21);
      this.homeComboBox.TabIndex = 4;
      this.toolTip1.SetToolTip((Control)this.homeComboBox, componentResourceManager.GetString("homeComboBox.ToolTip"));
      this.homeComboBox.SelectedIndexChanged += new EventHandler(this.homeComboBox_SelectedIndexChanged);
      this.pulseRateComboBox.AutoCompleteCustomSource.AddRange(new string[1]
      {
        "50"
      });
      this.pulseRateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.pulseRateComboBox.FormattingEnabled = true;
      this.pulseRateComboBox.Items.AddRange(new object[1]
      {
        (object) "50"
      });
      this.pulseRateComboBox.Location = new Point(191, 0);
      this.pulseRateComboBox.Name = "pulseRateComboBox";
      this.pulseRateComboBox.Size = new Size(45, 21);
      this.pulseRateComboBox.TabIndex = 10;
      this.toolTip1.SetToolTip((Control)this.pulseRateComboBox, "Selects the rate at which pulses are sent to the servo.");
      this.pulseRateComboBox.SelectedIndexChanged += new EventHandler(this.change);
      this.servoNameTextBox.Location = new Point(31, 1);
      this.servoNameTextBox.MaxLength = 16;
      this.servoNameTextBox.Name = "servoNameTextBox";
      this.servoNameTextBox.Size = new Size(83, 20);
      this.servoNameTextBox.TabIndex = 0;
      this.servoNameTextBox.TextChanged += new EventHandler(this.change);
      this.pulseRateLabel.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.pulseRateLabel.Location = new Point(200, 0);
      this.pulseRateLabel.Name = "pulseRateLabel";
      this.pulseRateLabel.Padding = new Padding(0, 0, 1, 0);
      this.pulseRateLabel.Size = new Size(36, 20);
      this.pulseRateLabel.TabIndex = 11;
      this.pulseRateLabel.Text = "0";
      this.pulseRateLabel.TextAlign = ContentAlignment.MiddleRight;
      this.speedUpDown.Location = new Point(512, 1);
      this.speedUpDown.MantissaBits = 5;
      RoundingNumericUpDown roundingNumericUpDown1 = this.speedUpDown;
      int[] bits1 = new int[4];
      bits1[0] = 4000;
      Decimal num1 = new Decimal(bits1);
      roundingNumericUpDown1.Maximum = num1;
      RoundingNumericUpDown roundingNumericUpDown2 = this.speedUpDown;
      int[] bits2 = new int[4];
      bits2[0] = 1;
      Decimal num2 = new Decimal(bits2);
      roundingNumericUpDown2.MinimumIncrement = num2;
      this.speedUpDown.Name = "speedUpDown";
      this.speedUpDown.Size = new Size(60, 20);
      this.speedUpDown.TabIndex = 6;
      this.speedUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.speedUpDown, componentResourceManager.GetString("speedUpDown.ToolTip"));
      this.speedUpDown.ValueChanged += new EventHandler(this.change);
      this.speedUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      this.rangeUpDown.DecimalPlaces = 2;
      this.rangeUpDown.Increment = new Decimal(new int[4]
      {
        3175,
        0,
        0,
        131072
      });
      this.rangeUpDown.Location = new Point(720, 1);
      this.rangeUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown3 = this.rangeUpDown;
      int[] bits3 = new int[4];
      bits3[0] = 4000;
      Decimal num3 = new Decimal(bits3);
      roundingNumericUpDown3.Maximum = num3;
      RoundingNumericUpDown roundingNumericUpDown4 = this.rangeUpDown;
      int[] bits4 = new int[4];
      bits4[0] = 150;
      Decimal num4 = new Decimal(bits4);
      roundingNumericUpDown4.Minimum = num4;
      RoundingNumericUpDown roundingNumericUpDown5 = this.rangeUpDown;
      int[] bits5 = new int[4];
      bits5[0] = 1;
      Decimal num5 = new Decimal(bits5);
      roundingNumericUpDown5.MinimumIncrement = num5;
      this.rangeUpDown.Name = "rangeUpDown";
      this.rangeUpDown.Size = new Size(70, 20);
      this.rangeUpDown.TabIndex = 9;
      this.rangeUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.rangeUpDown, "Sets the range of the servo for 8-bit commands, in μs.\r\nThe maximum and minimum positions of the servo\r\nwhen using the Mini-SSC protocol are given by the\r\nneutral position plus or minus the range.");
      this.rangeUpDown.Value = new Decimal(new int[4]
      {
        47625,
        0,
        0,
        131072
      });
      this.rangeUpDown.ValueChanged += new EventHandler(this.change);
      this.rangeUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      this.neutralUpDown.DecimalPlaces = 2;
      this.neutralUpDown.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
      this.neutralUpDown.Increment = new Decimal(new int[4]
      {
        25,
        0,
        0,
        131072
      });
      this.neutralUpDown.Location = new Point(644, 1);
      this.neutralUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown6 = this.neutralUpDown;
      int[] bits6 = new int[4];
      bits6[0] = 4000;
      Decimal num6 = new Decimal(bits6);
      roundingNumericUpDown6.Maximum = num6;
      RoundingNumericUpDown roundingNumericUpDown7 = this.neutralUpDown;
      int[] bits7 = new int[4];
      bits7[0] = 50;
      Decimal num7 = new Decimal(bits7);
      roundingNumericUpDown7.Minimum = num7;
      RoundingNumericUpDown roundingNumericUpDown8 = this.neutralUpDown;
      int[] bits8 = new int[4];
      bits8[0] = 1;
      Decimal num8 = new Decimal(bits8);
      roundingNumericUpDown8.MinimumIncrement = num8;
      this.neutralUpDown.Name = "neutralUpDown";
      this.neutralUpDown.Size = new Size(70, 20);
      this.neutralUpDown.TabIndex = 8;
      this.neutralUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.neutralUpDown, "Sets the neutral position for 8-bit commands, in μs.");
      RoundingNumericUpDown roundingNumericUpDown9 = this.neutralUpDown;
      int[] bits9 = new int[4];
      bits9[0] = 1500;
      Decimal num9 = new Decimal(bits9);
      roundingNumericUpDown9.Value = num9;
      this.neutralUpDown.ValueChanged += new EventHandler(this.change);
      this.neutralUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      this.homeUpDown.DecimalPlaces = 2;
      this.homeUpDown.Increment = new Decimal(new int[4]
      {
        25,
        0,
        0,
        131072
      });
      this.homeUpDown.Location = new Point(436, 1);
      this.homeUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown10 = this.homeUpDown;
      int[] bits10 = new int[4];
      bits10[0] = 4000;
      Decimal num10 = new Decimal(bits10);
      roundingNumericUpDown10.Maximum = num10;
      RoundingNumericUpDown roundingNumericUpDown11 = this.homeUpDown;
      int[] bits11 = new int[4];
      bits11[0] = 50;
      Decimal num11 = new Decimal(bits11);
      roundingNumericUpDown11.Minimum = num11;
      RoundingNumericUpDown roundingNumericUpDown12 = this.homeUpDown;
      int[] bits12 = new int[4];
      bits12[0] = 1;
      Decimal num12 = new Decimal(bits12);
      roundingNumericUpDown12.MinimumIncrement = num12;
      this.homeUpDown.Name = "homeUpDown";
      this.homeUpDown.Size = new Size(70, 20);
      this.homeUpDown.TabIndex = 5;
      this.homeUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.homeUpDown, "The servo will go to this position on power-up or when\r\nan error is encountered.  For output ports, this value just\r\ndetermines whether this port will be high (≥1500) or low (<1500).");
      RoundingNumericUpDown roundingNumericUpDown13 = this.homeUpDown;
      int[] bits13 = new int[4];
      bits13[0] = 1500;
      Decimal num13 = new Decimal(bits13);
      roundingNumericUpDown13.Value = num13;
      this.homeUpDown.ValueChanged += new EventHandler(this.change);
      this.homeUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      RoundingNumericUpDown roundingNumericUpDown14 = this.maxUpDown;
      int[] bits14 = new int[4];
      bits14[0] = 16;
      Decimal num14 = new Decimal(bits14);
      roundingNumericUpDown14.Increment = num14;
      this.maxUpDown.Location = new Point(308, 1);
      this.maxUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown15 = this.maxUpDown;
      int[] bits15 = new int[4];
      bits15[0] = 2500;
      Decimal num15 = new Decimal(bits15);
      roundingNumericUpDown15.Maximum = num15;
      RoundingNumericUpDown roundingNumericUpDown16 = this.maxUpDown;
      int[] bits16 = new int[4];
      bits16[0] = 50;
      Decimal num16 = new Decimal(bits16);
      roundingNumericUpDown16.Minimum = num16;
      RoundingNumericUpDown roundingNumericUpDown17 = this.maxUpDown;
      int[] bits17 = new int[4];
      bits17[0] = 1;
      Decimal num17 = new Decimal(bits17);
      roundingNumericUpDown17.MinimumIncrement = num17;
      this.maxUpDown.Name = "maxUpDown";
      this.maxUpDown.Size = new Size(60, 20);
      this.maxUpDown.TabIndex = 3;
      this.maxUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.maxUpDown, "Sets the maximum allowed position, in μs.");
      RoundingNumericUpDown roundingNumericUpDown18 = this.maxUpDown;
      int[] bits18 = new int[4];
      bits18[0] = 2000;
      Decimal num18 = new Decimal(bits18);
      roundingNumericUpDown18.Value = num18;
      this.maxUpDown.ValueChanged += new EventHandler(this.change);
      this.maxUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      RoundingNumericUpDown roundingNumericUpDown19 = this.minUpDown;
      int[] bits19 = new int[4];
      bits19[0] = 16;
      Decimal num19 = new Decimal(bits19);
      roundingNumericUpDown19.Increment = num19;
      this.minUpDown.Location = new Point(242, 1);
      this.minUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown20 = this.minUpDown;
      int[] bits20 = new int[4];
      bits20[0] = 2500;
      Decimal num20 = new Decimal(bits20);
      roundingNumericUpDown20.Maximum = num20;
      RoundingNumericUpDown roundingNumericUpDown21 = this.minUpDown;
      int[] bits21 = new int[4];
      bits21[0] = 50;
      Decimal num21 = new Decimal(bits21);
      roundingNumericUpDown21.Minimum = num21;
      RoundingNumericUpDown roundingNumericUpDown22 = this.minUpDown;
      int[] bits22 = new int[4];
      bits22[0] = 1;
      Decimal num22 = new Decimal(bits22);
      roundingNumericUpDown22.MinimumIncrement = num22;
      this.minUpDown.Name = "minUpDown";
      this.minUpDown.Size = new Size(60, 20);
      this.minUpDown.TabIndex = 2;
      this.minUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.minUpDown, "Sets the minimum allowed position, in μs.");
      RoundingNumericUpDown roundingNumericUpDown23 = this.minUpDown;
      int[] bits23 = new int[4];
      bits23[0] = 992;
      Decimal num23 = new Decimal(bits23);
      roundingNumericUpDown23.Value = num23;
      this.minUpDown.ValueChanged += new EventHandler(this.change);
      this.minUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      this.accelerationUpDown.Location = new Point(578, 1);
      this.accelerationUpDown.MantissaBits = 0;
      RoundingNumericUpDown roundingNumericUpDown24 = this.accelerationUpDown;
      int[] bits24 = new int[4];
      bits24[0] = (int)byte.MaxValue;
      Decimal num24 = new Decimal(bits24);
      roundingNumericUpDown24.Maximum = num24;
      RoundingNumericUpDown roundingNumericUpDown25 = this.accelerationUpDown;
      int[] bits25 = new int[4];
      bits25[0] = 1;
      Decimal num25 = new Decimal(bits25);
      roundingNumericUpDown25.MinimumIncrement = num25;
      this.accelerationUpDown.Name = "accelerationUpDown";
      this.accelerationUpDown.Size = new Size(60, 20);
      this.accelerationUpDown.TabIndex = 7;
      this.accelerationUpDown.TextAlign = HorizontalAlignment.Right;
      this.toolTip1.SetToolTip((Control)this.accelerationUpDown, "Sets the acceleration limit of the servo.\r\nA value of 0 indicates unlimited acceleration.  This value\r\nmay also be changed at run time.");
      this.accelerationUpDown.ValueChanged += new EventHandler(this.change);
      this.accelerationUpDown.KeyPress += new KeyPressEventHandler(this.changeStart);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control)this.pulseRateComboBox);
      this.Controls.Add((Control)this.pulseRateLabel);
      this.Controls.Add((Control)this.servoNameTextBox);
      this.Controls.Add((Control)this.homeComboBox);
      this.Controls.Add((Control)this.modeComboBox);
      this.Controls.Add((Control)this.speedUpDown);
      this.Controls.Add((Control)this.rangeUpDown);
      this.Controls.Add((Control)this.neutralUpDown);
      this.Controls.Add((Control)this.homeUpDown);
      this.Controls.Add((Control)this.maxUpDown);
      this.Controls.Add((Control)this.minUpDown);
      this.Controls.Add((Control)this.accelerationUpDown);
      this.Controls.Add((Control)this.servoNumberLabel);
      this.Margin = new Padding(0, 2, 0, 2);
      this.Name = "ServoControl";
      this.Size = new Size(800, 21);
      this.speedUpDown.EndInit();
      this.rangeUpDown.EndInit();
      this.neutralUpDown.EndInit();
      this.homeUpDown.EndInit();
      this.maxUpDown.EndInit();
      this.minUpDown.EndInit();
      this.accelerationUpDown.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

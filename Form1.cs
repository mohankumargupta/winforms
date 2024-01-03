using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using Pololu.Usc;
using Pololu.MaestroControlCenter;
using System.Text;
using System.ComponentModel;
using Pololu.Usc.Sequencer;
using Pololu.UsbWrapper;
using System.IO;
using Pololu.Usc.Bytecode;
using System.Diagnostics;
using System.IO.Ports;


namespace winforms;

public partial class Form1 : Form
{
    public bool miniMaestro => this.usc != null && this.usc.servoCount > (byte)6;
    public SerialPort serialPort;

    public Form1()
    {
        InitializeComponent();
    }

    private void setScriptTextBoxKeywords()
    {
        this.scriptTextBox.removeAllKeywordsOfColor(1);
        this.scriptTextBox.removeAllKeywordsOfColor(2);
        //foreach (string name in Enum.GetNames(typeof(OpCode)))
        //    this.scriptTextBox.addKeyword(name, 1);
        //foreach (string name in Enum.GetNames(typeof(Keyword)))
        //    this.scriptTextBox.addKeyword(name, 2);
        this.scriptTextBox.addCommentType("#", "\n");
    }

    private void updateKeywords(object o, EventArgs e)
    {
        this.scriptTextBox.removeAllKeywordsOfColor(3);
        for (Match match = new Regex("(?<!\\S)sub\\s+(\\S+)", RegexOptions.IgnoreCase).Match(this.scriptTextBox.Text); match.Success; match = match.NextMatch())
            this.scriptTextBox.addKeyword(match.Groups[1].Value, 3);
        this.scriptTextBox.removeAllKeywordsOfColor(4);
        for (Match match = new Regex("(\\S+):(?!\\S)", RegexOptions.IgnoreCase).Match(this.scriptTextBox.Text); match.Success; match = match.NextMatch())
        {
            this.scriptTextBox.addKeyword(match.Groups[0].Value, 4);
            this.scriptTextBox.addKeyword(match.Groups[1].Value, 4);
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        this.Text = Form1.title;
        this.updateControlsForModel((byte)6);
    }

    private String? openCOMPort(string comport)
    {
        string[] ports = System.IO.Ports.SerialPort.GetPortNames();
        for (int i = 0; i < ports.Length; i++)
        {
            string port = ports[i].ToUpper();

            if (port.StartsWith(comport))
            {
                System.Console.WriteLine(port);
                serialPort = new SerialPort(port, 9600);
                serialPort.Open();
                return port;
            }
        }
        return null;
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
        String COMPort = "COM4";
        String? port = openCOMPort(COMPort);

        this.deviceList.Items.Clear();
        this.deviceList.DisplayMember = "text";
        //this.deviceList.Items.Add((object)DeviceListItem.CreateDummyItem("Not Connected"));
        if (port != null)
        {
            this.deviceList.Items.Add(port);
        }



        //this.updateDeviceListContents();

        /*
        if (!Usb.supportsNotify)
            new Thread(new ThreadStart(this.backgroundUpdateDeviceList)).Start();
        try
        {
            if (Usb.supportsNotify)
                Usb.notificationRegister(Pololu.Usc.Usc.deviceInterfaceGuid, this.Handle);
        }
        catch (Exception ex)
        {
            Form1.displayException(ex);
        }
        try
        {
            this.updateDeviceListContents();
            if (this.deviceCount == 1)
                this.connectToDevice(0);
            this.updateDeviceListSelectedIndex();
        }
        catch (Exception ex)
        {
            Form1.displayException(ex, "There was an error while starting up.");
        }
        */
        //this.updateFormFromDeviceAndRegistry();
        new Thread(new ThreadStart(this.backgroundUpdateStatus)).Start();
    }

    private static string title => "Pololu Maestro Control Center";

    private void resetToFactorySettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!Form1.confirm("This will reset all of your device's settings back to their default values.\nYou will lose your custom settings.  Are you sure you want to continue?\n", "Reset Confirmation"))
            return;
        this.applyProgressBar.Visible = true;
        this.applyProgressBar.Value = 33;
        this.usc.restoreDefaultConfiguration();
        this.applyProgressBar.Value = 66;
        this.usc.eraseScript();
        this.applyProgressBar.Value = 100;
        this.ReloadButton_Click((object)null, (EventArgs)null);
        this.applyProgressBar.Value = this.applyProgressBar.Minimum;
        this.applyProgressBar.Visible = false;
        Form1.message("Your device's settings have been reset to their default values.", "Reset Complete");
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
        this.applyProgressBar.Value = 0;
        this.applyProgressBar.Visible = true;
        this.Cursor = Cursors.WaitCursor;
        try
        {
            UscSettings uscSettings = this.getUscSettings();
            this.applyProgressBar.Value = 33;
            List<string> warnings = new List<string>();
            this.usc.fixSettings(uscSettings, warnings);
            if (warnings.Count > 0)
            {
                string str1 = "There were problems with your settings:\n" + "\n";
                foreach (string str2 in warnings)
                    str1 = str1 + str2 + "\n";
                if (!Form1.confirm(str1 + "\n" + "These problems will be automatically fixed.\n" + "Do you still want to apply these settings to the device?", "Settings Problems"))
                    return;
            }
            this.cachedParameters = uscSettings;
            this.usc.setUscSettings(this.cachedParameters, !this.scriptMatchesDevice);
            this.scriptMatchesDevice = true;
            if (warnings.Count > 0)
                this.setUscSettings(uscSettings, false);
            this.applyProgressBar.Value = 66;
            this.usc.reinitialize();
            this.applyProgressBar.Value = 100;
            this.ApplyButton.Enabled = false;
            this.reloadSettingsToolStripMenuItem.Enabled = false;
            this.cachedParametersChanged();
        }
        catch (Exception ex)
        {
            Form1.displayException(ex, "Error applying parameters.");
            this.suppressEvents = false;
        }
        finally
        {
            this.applyProgressBar.Value = 0;
            this.applyProgressBar.Visible = false;
            this.Cursor = Cursors.Default;
            /*
            if (AppInfo.compiledForLinux)
            {
                bool enabled = this.ApplyButton.Enabled;
                this.ApplyButton.Enabled = true;
                this.ApplyButton.Cursor = Cursors.Default;
                this.ApplyButton.Enabled = enabled;
            }
            */
        }
    }

    private void ReloadButton_Click(object sender, EventArgs e)
    {
        this.updateFormFromDeviceAndRegistry();
    }

    private void EnableApplyButton(object sender, KeyPressEventArgs e)
    {
        this.EnableApplyButton(sender, (EventArgs)e);
    }

    private void EnableApplyButton(object sender, EventArgs e)
    {
        if (this.suppressEvents)
            return;
        this.ApplyButton.Enabled = true;
        this.reloadSettingsToolStripMenuItem.Enabled = true;
        this.applyProgressBar.Value = 0;
    }

    private void updateFormFromDeviceAndRegistry()
    {
        this.restartDeviceToolStripMenuItem.Enabled = this.usc != null;
        this.suppressEvents = true;
        try
        {
            this.statusTab.Enabled = this.errorsTab.Enabled = this.settingsTab.Enabled = this.serialSettingsTab.Enabled = this.sequenceTab.Enabled = this.scriptTab.Enabled = this.saveFrameButton.Enabled = this.restartDeviceToolStripMenuItem.Enabled = this.usc != null;
            if (this.usc != null)
            {
                try
                {
                    this.cachedParameters = this.usc.getUscSettings();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading settings from the device.", ex);
                }
                try
                {
                    this.setUscSettings(this.cachedParameters, true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error applying settings to the form.", ex);
                }
                bool scriptInconsistent = this.cachedParameters.scriptInconsistent;
                this.cachedParametersChanged();
                this.updateControlsForModel(this.cachedParameters.servoCount);
                this.resetStatusTabTargetStates();
                if (scriptInconsistent)
                {
                    Form1.warning("The current script for this device could not be found.\nThis could be because it was programmed on another computer.\nWhen you click the 'Apply settings to device' button, the script\ncurrently in the device will be replaced.\nTo avoid this problem, save a settings file on the other computer.", "Script source code missing");
                    this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
                    this.scriptMatchesDevice = false;
                }
                else
                {
                    this.disableApplyButton();
                    this.scriptMatchesDevice = true;
                }
            }
            else
                this.disableApplyButton();
        }
        catch (Exception ex)
        {
            Form1.displayException(ex);
            this.disconnect();
            this.updateFormFromDeviceAndRegistry();
            this.updateDeviceListSelectedIndex();
        }
        this.suppressEvents = false;
    }

    private void resetStatusTabTargetStates()
    {
        foreach (ServoStatusControl servoStatusControl in this.servoStatusControls)
            servoStatusControl.resetTargetState();
    }

    private void updateControlsForModel(byte servoCount)
    {
        /*
        for (byte index = 0; (int)index < (int)servoCount; ++index)
        {
            this.servoControls[(int)index].Visible = true;
            this.servoStatusControls[(int)index].Visible = true;
        }
        for (byte index = servoCount; (int)index < this.servoControls.Length; ++index)
        {
            this.servoControls[(int)index].Visible = false;
            this.servoStatusControls[(int)index].Visible = false;
        }
        this.microMaestroAdvancedBox.Visible = servoCount == (byte)6;
        this.miniMaestroAdvancedBox.Visible = servoCount > (byte)6;
        this.enablePullupsCheckBox.Visible = servoCount > (byte)18;
        this.pwmGroupBox.Visible = servoCount > (byte)6;
        if (servoCount == (byte)12)
            this.pwmCheckBox.Text = "Enable PWM on channel 8";
        else
            this.pwmCheckBox.Text = "Enable PWM on channel 12";
        this.servoStatusFlowLayoutPanel.Height = this.servoStatusFlowLayoutPanel.PreferredSize.Height;
        this.servoControlFlowLayoutPanel.Height = this.servoControlFlowLayoutPanel.PreferredSize.Height;
        */
    }

    private void disableApplyButton()
    {
        this.ApplyButton.Enabled = false;
        this.reloadSettingsToolStripMenuItem.Enabled = false;
    }

    private void cachedParametersChanged()
    {
        this.updateStatusTabWithCachedParameters();
        this.updateNames();
        if (this.currentProgram != null)
            this.scriptBytesLabel.Text = "Script: " + (object)this.currentProgram.getByteList().Count + " of " + (object)this.usc.maxScriptLength + " bytes used";
        else
            this.scriptBytesLabel.Text = "";
        this.autoBaudWarningShown = false;
        this.resetPWM();
    }

    private void updateNames()
    {
        for (int index = 0; index < (int)this.usc.servoCount; ++index)
            this.servoStatusControls[index].servoName = this.servoControls[index].servoName;
    }

    private void updatePulseRates()
    {
        List<int> pulseRates = new List<int>();
        Decimal num = this.miniMaestro ? this.miniMaestroServoPeriod.Value : this.periodUpDown.Value;
        Decimal d1 = 1000M / num;
        pulseRates.Add((int)Decimal.Round(d1, MidpointRounding.AwayFromZero));
        if (this.miniMaestro && this.servoMultiplier.Value > 1M)
        {
            Decimal d2 = 1000M / (num * this.servoMultiplier.Value);
            pulseRates.Add((int)Decimal.Round(d2, MidpointRounding.AwayFromZero));
        }
        foreach (ServoControl servoControl in this.servoControls)
            servoControl.setPulseRateOptions(pulseRates);
    }

    private void periodMultiplier_ValueChanged(object sender, EventArgs e)
    {
        this.EnableApplyButton(sender, e);
        this.updatePulseRates();
    }

    private void miniMaestroServoPeriod_ValueChanged(object sender, EventArgs e)
    {
        this.EnableApplyButton(sender, e);
        this.updatePulseRates();
    }

    private void resetPWM()
    {
        bool suppressEvents = this.suppressEvents;
        this.suppressEvents = true;
        this.pwmCheckBox.Checked = false;
        this.pwmPeriod.Value = this.pwmPeriod.Maximum - 64M;
        this.pwmDutyCycle.Maximum = this.pwmPeriod.Maximum - 64M;
        this.pwmDutyCycle.Value = this.pwmPeriod.Value / 2M;
        this.pwmPeriod.Increment = 64M;
        this.pwmDutyCycle.Increment = this.pwmPeriod.Increment / 4M;
        this.suppressEvents = suppressEvents;
    }

    private void updatePWM(object sender, EventArgs e)
    {
        if (this.usc == null || this.suppressEvents)
            return;
        int num1 = 4;
        if (this.pwmPeriod.Value > 1024M)
            num1 = 16;
        if (this.pwmPeriod.Value > 4096M)
            num1 = 64;
        this.suppressEvents = true;
        this.pwmPeriod.Increment = (Decimal)num1;
        this.pwmDutyCycle.Increment = (Decimal)(num1 / 4);
        if (this.pwmPeriod.Value % (Decimal)num1 != 0M)
        {
            Decimal num2 = this.pwmPeriod.Value + (Decimal)num1 - this.pwmPeriod.Value % (Decimal)num1;
            if (num2 > this.pwmPeriod.Maximum)
                num2 = this.pwmPeriod.Maximum;
            this.pwmPeriod.Value = num2;
        }
        if (this.pwmDutyCycle.Value % (Decimal)(num1 / 4) != 0M)
        {
            Decimal num3 = (Decimal)(((int)this.pwmDutyCycle.Value + num1 / 8) / (num1 / 4) * (num1 / 4));
            if (num3 > this.pwmDutyCycle.Maximum)
                num3 = this.pwmDutyCycle.Maximum;
            this.pwmDutyCycle.Value = num3;
        }
        switch ((int)this.pwmPeriod.Value)
        {
            case 1024:
                this.pwmDutyCycle.Maximum = 1023M;
                break;
            case 4096:
                this.pwmDutyCycle.Maximum = 4095M;
                break;
            case 16384:
                this.pwmDutyCycle.Maximum = 16383M;
                break;
            default:
                this.pwmDutyCycle.Maximum = this.pwmPeriod.Value;
                break;
        }
        this.suppressEvents = false;
        if (this.pwmCheckBox.Checked)
        {
            if (this.cachedParameters.channelSettings.Count == 12 && this.cachedParameters.channelSettings[8].mode != ChannelMode.Output || this.cachedParameters.channelSettings.Count > 12 && this.cachedParameters.channelSettings[12].mode != ChannelMode.Output)
            {
                Form1.displayException("The channel must be configured as an output to use PWM.");
                this.suppressEvents = true;
                this.pwmCheckBox.Checked = false;
                this.suppressEvents = false;
            }
            else
                this.usc.setPWM((ushort)this.pwmDutyCycle.Value, (ushort)this.pwmPeriod.Value);
        }
        else
        {
            if (sender != this.pwmCheckBox)
                return;
            this.usc.disablePWM();
        }
    }

    private void updateStatusTabWithCachedParameters()
    {
        for (byte index = 0; (int)index < (int)this.cachedParameters.servoCount; ++index)
        {
            ServoStatusControl servoStatusControl = this.servoStatusControls[(int)index];
            ChannelSetting channelSetting = this.cachedParameters.channelSettings[(int)index];
            servoStatusControl.supressEvents = true;
            servoStatusControl.mode = channelSetting.mode;
            servoStatusControl.min = channelSetting.minimum;
            servoStatusControl.max = channelSetting.maximum;
            servoStatusControl.home = (int)channelSetting.home < (int)channelSetting.minimum || (int)channelSetting.home > (int)channelSetting.maximum ? (ushort)(((int)channelSetting.minimum + (int)channelSetting.maximum) / 2) : channelSetting.home;
            servoStatusControl.supressEvents = false;
        }
    }

    private void updateServoMaxLimit()
    {
        if (this.usc == null || this.usc.servoCount == (byte)6)
        {
            ushort num = (ushort)(this.periodUpDown.Value * 4000M / this.servosAvailableUpDown.Value - 200M);
            foreach (ServoControl servoControl in this.servoControls)
                servoControl.maxLimit = num;
        }
        else
        {
            foreach (ServoControl servoControl in this.servoControls)
                servoControl.maxLimit = (ushort)16320;
        }
    }

    private void updateServosOnlyAllowIO()
    {
        if (this.usc != null && this.usc.servoCount == (byte)6)
        {
            int index;
            for (index = 0; index < (int)this.servosAvailableUpDown.Value && index < this.servoControls.Length; ++index)
                this.servoControls[index].onlyAllowIO = false;
            for (; index < this.servoControls.Length; ++index)
                this.servoControls[index].onlyAllowIO = true;
        }
        else
        {
            for (int index = 0; index < this.servoControls.Length; ++index)
                this.servoControls[index].onlyAllowIO = false;
        }
    }

    private void setPeriodMax()
    {
        this.periodUpDown.Maximum = Pololu.Usc.Usc.periodToMicroseconds((ushort)byte.MaxValue, (byte)this.servosAvailableUpDown.Value) / 1000M;
    }

    private void servosAvailableUpDown_ValueChanged(object sender, EventArgs e)
    {
        this.updateServosOnlyAllowIO();
        this.setPeriodMax();
        this.updateServoMaxLimit();
        this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
    }

    private void periodUpDown_ValueChanged(object sender, EventArgs e)
    {
        this.updateServoMaxLimit();
        this.updatePulseRates();
        if (this.suppressEvents)
            return;
        this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
    }

    public static string convertExceptionToString(Exception exception)
    {
        StringBuilder stringBuilder = new StringBuilder();
        do
        {
            stringBuilder.Append(exception.Message + "  ");
            if (exception is Win32Exception)
                stringBuilder.Append("Error code 0x" + ((Win32Exception)exception).NativeErrorCode.ToString("x") + ".  ");
            exception = exception.InnerException;
        }
        while (exception != null);
        return stringBuilder.ToString().Trim();
    }

    public static void displayException(string broadErrorSentence)
    {
        Form1.displayException(new Exception(broadErrorSentence));
    }

    public static void displayException(Exception exception, string broadErrorSentence)
    {
        Form1.displayException(new Exception(broadErrorSentence, exception));
    }

    public static void displayException(Exception exception)
    {
        int num = (int)MessageBox.Show(Form1.convertExceptionToString(exception), Form1.title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    public static bool confirm(string question, string caption)
    {
        return DialogResult.OK == MessageBox.Show(question, Form1.title + " " + caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
    }

    public static void message(string text, string caption)
    {
        int num = (int)MessageBox.Show(text, Form1.title + " " + caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    public static void warning(string text, string caption)
    {
        int num = (int)MessageBox.Show(text, Form1.title + " " + caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static bool command(string command, string caption)
    {
        return DialogResult.OK == MessageBox.Show(command, Form1.title + " " + caption, MessageBoxButtons.OKCancel);
    }

    private void displaySettingWarnings(
      List<string> warnings,
      string beforeSentence,
      string afterSentence)
    {
        if (warnings.Count == 0)
            return;
        string str = beforeSentence + "\n";
        foreach (string warning in warnings)
            str = str + warning + "\n";
        Form1.warning(str + "\n" + afterSentence, "Settings Warning");
    }

    private void frameTimerTick(object o, EventArgs e)
    {
        int index = this.currentFrame + 1;
        if (index >= this.selectedSequence.frames.Count)
        {
            if (this.loopBox.Checked)
            {
                index = 0;
            }
            else
            {
                this.stopSequence();
                return;
            }
        }
        this.currentFrame = index;
        ushort lengthMs = this.selectedSequence.frames[index].length_ms;
        if (lengthMs == (ushort)0)
        {
            this.frameTimer.Interval = 1;
        }
        else
        {
            this.frameTimer.Interval = (int)lengthMs;
            this.markAndLoadCurrentFrame();
        }
    }

    private void markAndLoadCurrentFrame()
    {
        if (this.currentFrame >= this.frameListView.Items.Count || this.currentFrame >= this.selectedSequence.frames.Count)
            return;
        for (int index = 0; index < this.frameListView.Items.Count; ++index)
            this.frameListView.Items[index].SubItems[0].Text = this.currentFrame != index ? "" : ">";
        this.frameListView.Invalidate();
        this.loadFrame(this.selectedSequence.frames[this.currentFrame]);
    }

    private void unmarkCurrentFrame()
    {
        if (this.currentFrame >= this.frameListView.Items.Count)
            return;
        for (int index = 0; index < this.frameListView.Items.Count; ++index)
            this.frameListView.Items[index].SubItems[0].Text = "";
        this.frameListView.Invalidate();
    }

    private void sequenceList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.suppressEvents)
            return;
        this.stopSequence();
        this.updateSelectedSequence();
        if (this.sequenceList.SelectedIndex >= this.sequences.Count)
            return;
        this.refreshSequencer();
    }

    private void updateSelectedSequence()
    {
        if (this.sequenceList.SelectedIndex >= this.sequences.Count || this.sequenceList.SelectedIndex == -1)
            this.selectedSequence = null;
        else
            this.selectedSequence = this.sequences[this.sequenceList.SelectedIndex];
    }

    private void refreshSequencer(Sequence newSelectedSequence)
    {
        this.stopSequence();
        this.sequences.Sort((Comparison<Sequence>)((s, t) => s.name.CompareTo(t.name)));
        this.suppressEvents = true;
        this.sequenceList.Items.Clear();
        foreach (Sequence sequence in this.sequences)
        {
            this.sequenceList.Items.Add((object)sequence.name);
            if (sequence == newSelectedSequence)
                this.sequenceList.SelectedIndex = this.sequenceList.Items.Count - 1;
        }
        if (this.sequenceList.SelectedIndex < 0 && this.sequenceList.Items.Count > 0)
            this.sequenceList.SelectedIndex = this.sequenceList.Items.Count - 1;
        this.suppressEvents = false;
        this.updateSelectedSequence();
        this.frameListView.Items.Clear();
        if (this.selectedSequence != null)
        {
            foreach (Frame frame in this.selectedSequence.frames)
                this.frameListView.Items.Add(this.frameToListViewItem(frame));
        }
        this.refreshSaveFrameButton();
    }

    private void refreshSequencer() => this.refreshSequencer(this.selectedSequence);

    private void refreshSaveFrameButton()
    {
        if (this.selectedSequence == null)
            this.saveFrameButton.Text = "&Save Frame 0";
        else
            this.saveFrameButton.Text = "&Save Frame " + (object)this.selectedSequence.frames.Count;
    }

    private ListViewItem frameToListViewItem(Frame frame)
    {
        return new ListViewItem("")
        {
            SubItems = {
          frame.name,
          frame.length_ms.ToString()
        }
        };
    }

    private void refreshFrame(int index)
    {
        if (index < 0 || index >= this.frameListView.Items.Count)
            return;
        ListViewItem listViewItem1 = this.frameToListViewItem(this.selectedSequence.frames[index]);
        ListViewItem listViewItem2 = this.frameListView.Items[index];
        listViewItem2.SubItems[1].Text = listViewItem1.SubItems[1].Text;
        listViewItem2.SubItems[2].Text = listViewItem1.SubItems[2].Text;
    }

    private void newSequenceButton_Click(object sender, EventArgs e)
    {
        Sequence newSelectedSequence = new Sequence("Sequence " + (object)this.sequences.Count);
        this.sequences.Add(newSelectedSequence);
        this.refreshSequencer(newSelectedSequence);
        this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
    }

    private void renameSequenceButton_Click(object sender, EventArgs e)
    {
        this.stopSequence();
        if (this.sequences.Count == 0)
        {
            Form1.displayException("There are no sequences.");
        }
        else
        {
            int num = (int)SequenceEditWindow.go(this.selectedSequence);
            this.refreshSequencer();
            this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
        }
    }

    private void deleteSequenceButton_Click(object sender, EventArgs e)
    {
        if (this.sequences.Count == 0)
        {
            Form1.displayException("There are no sequences.");
        }
        else
        {
            int selectedIndex = this.sequenceList.SelectedIndex;
            if (!Form1.confirm("Really delete sequence \"" + this.sequences[selectedIndex].name + "\"?", "Delete Sequence Confirmation"))
                return;
            this.sequences.RemoveAt(selectedIndex);
            this.refreshSequencer();
            this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
            this.frameListView.Focus();
        }
    }

    private void loadFrameButton_Click(object sender, EventArgs e)
    {
        this.stopSequence();
        this.frameListView.Focus();
        List<Frame> selectedFrames = this.selectedFrames;
        if (selectedFrames.Count == 0)
            Form1.displayException("To load a frame, you must select it first.  To create a new frame, use the \"Save frame\" button.");
        else if (selectedFrames.Count > 1)
            Form1.displayException("To load a frame, you must select exactly one frame.  You have selected multiple frames.");
        else
            this.loadFrame(selectedFrames[0]);
    }

    private void loadFrame(Frame frame)
    {
        if (frame == null)
            return;
        for (byte index = 0; (int)index < (int)this.usc.servoCount; ++index) { }
        //this.usc.setTarget(index, frame[(int)index]);
    }

    private void frameListView_DoubleClick(object sender, EventArgs e)
    {
        if (this.frameListView.SelectedIndices.Count != 1)
            return;
        this.editFrameButton_Click(sender, e);
    }

    private void editFrameButton_Click(object sender, EventArgs e)
    {
        this.stopSequence();
        this.frameListView.Focus();
        List<Frame> selectedFrames = this.selectedFrames;
        if (selectedFrames.Count == 0)
        {
            Form1.displayException("You must select a frame first.");
        }
        else
        {
            int[] dest = new int[this.frameListView.SelectedIndices.Count];
            this.frameListView.SelectedIndices.CopyTo((Array)dest, 0);
            if (FrameEditWindow.launch((IList<Frame>)selectedFrames) == DialogResult.Cancel)
                return;
            foreach (int index in dest)
                this.refreshFrame(index);
            this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
        }
    }

    private void deleteFrameButton_Click(object sender, EventArgs e) => this.removeFrames(true);

    private void removeFrames(bool fromButton)
    {
        this.stopSequence();
        this.frameListView.Focus();
        Sequence selectedSequence = this.selectedSequence;
        ListView.SelectedIndexCollection selectedIndices = this.frameListView.SelectedIndices;
        if (selectedIndices.Count == 0)
        {
            if (!fromButton)
                return;
            Form1.displayException("You must select a frame first.");
        }
        else
        {
            if (fromButton && selectedIndices.Count > 1 && !Form1.confirm("Really delete " + (object)selectedIndices.Count + " frames?", ""))
                return;
            int[] array = new int[selectedIndices.Count];
            for (int index = 0; index < array.Length; ++index)
                array[index] = selectedIndices[index];
            Array.Sort<int>(array);
            for (int index = array.Length - 1; index >= 0; --index)
            {
                this.frameListView.Items.RemoveAt(array[index]);
                selectedSequence.frames.RemoveAt(array[index]);
            }
            this.refreshSaveFrameButton();
            this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
        }
    }

    private void frameListView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            this.removeFrames(false);
            e.Handled = true;
        }
        if (e.Modifiers == Keys.Control)
        {
            if (e.KeyCode == Keys.A)
            {
                foreach (ListViewItem listViewItem in this.frameListView.Items)
                    listViewItem.Selected = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.C)
            {
                Frame.copyToClipboard(this.selectedFrames);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.X)
            {
                Frame.copyToClipboard(this.selectedFrames);
                this.removeFrames(false);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.V)
            {
                this.pasteFrames();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                this.moveFrames(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.moveFrames(false);
                e.Handled = true;
            }
        }
        if (e.Modifiers != Keys.Shift || e.KeyCode != Keys.Insert)
            return;
        this.pasteFrames();
        e.Handled = true;
    }

    private void frameUpButton_Click(object sender, EventArgs e) => this.moveFrames(true);

    private void frameDownButton_Click(object sender, EventArgs e) => this.moveFrames(false);

    private void moveFrames(bool up)
    {
        this.stopSequence();
        this.frameListView.Focus();
        if (this.frameListView.SelectedIndices.Count == 0)
        {
            Form1.displayException("You must select a frame first.");
        }
        else
        {
            int newIndex1 = int.MaxValue;
            int newIndex2 = int.MinValue;
            foreach (int selectedIndex in this.frameListView.SelectedIndices)
            {
                if (selectedIndex < newIndex1)
                    newIndex1 = selectedIndex;
                if (selectedIndex > newIndex2)
                    newIndex2 = selectedIndex;
            }
            if (newIndex2 - newIndex1 + 1 != this.frameListView.SelectedIndices.Count)
                Form1.displayException("To move multiple frames, all selected\nframes must be contiguous (no gaps).");
            else if (up)
                this.moveFrame(newIndex1 - 1, newIndex2);
            else
                this.moveFrame(newIndex2 + 1, newIndex1);
        }
    }

    private void pasteFrames()
    {
        try
        {
            int index;
            if (this.frameListView.SelectedIndices.Count == 0)
            {
                index = this.frameListView.Items.Count;
            }
            else
            {
                index = this.frameListView.SelectedIndices[0];
                foreach (int selectedIndex in this.frameListView.SelectedIndices)
                {
                    if (selectedIndex < index)
                        index = selectedIndex;
                }
            }
            List<Frame> fromClipboard = Frame.getFromClipboard();
            if (fromClipboard == null || fromClipboard.Count == 0)
                return;
            this.stopSequence();
            if (this.selectedSequence == null)
                this.newSequenceButton_Click((object)null, (EventArgs)null);
            Sequence selectedSequence = this.selectedSequence;
            if (selectedSequence == null)
                return;
            this.frameListView.SelectedIndices.Clear();
            int num = index;
            foreach (Frame frame in fromClipboard)
            {
                selectedSequence.frames.Insert(num, frame);
                this.frameListView.Items.Insert(num, this.frameToListViewItem(frame));
                this.frameListView.SelectedIndices.Add(num);
                ++num;
            }
            this.frameListView.FocusedItem = this.frameListView.Items[index];
            this.refreshSaveFrameButton();
        }
        catch (Exception ex)
        {
            Form1.displayException(ex, "There was an error pasting frames from clipboard.");
        }
        this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
    }

    private void moveFrame(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= this.frameListView.Items.Count || newIndex < 0 || newIndex >= this.frameListView.Items.Count)
            return;
        Sequence selectedSequence = this.selectedSequence;
        if (selectedSequence == null)
            return;
        ListViewItem listViewItem = this.frameListView.Items[oldIndex];
        this.frameListView.Items.RemoveAt(oldIndex);
        this.frameListView.Items.Insert(newIndex, listViewItem);
        Frame frame = selectedSequence.frames[oldIndex];
        selectedSequence.frames.RemoveAt(oldIndex);
        selectedSequence.frames.Insert(newIndex, frame);
        this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
    }

    private void playSequenceButton_Click(object sender, EventArgs e)
    {
        this.startSequenceAtBeginning();
    }

    private void stopSequence()
    {
        this.frameTimer.Enabled = false;
        this.unmarkCurrentFrame();
    }

    private void copyToSubroutinesButton_Click(object sender, EventArgs e)
    {
        if (this.sequences.Count == 0)
        {
            Form1.displayException("You have not created any sequences.");
        }
        else
        {
            string str = "### Sequence subroutines: ###";
            if (this.scriptTextBox.Text.Contains(str) && !Form1.command("Your script appears to already contain sequence\nsubroutines.  Are you sure you want to insert a\nnew copy?", "Copy to Subroutines"))
                return;
            this.scriptTextBox.Text = this.scriptTextBox.Text + "\n" + str + "\n\n" + Sequence.generateSubroutineList(this.enabledChannels, this.sequences);
            this.tabs.SelectedTab = this.scriptTab;
        }
    }

    private List<byte> enabledChannels
    {
        get
        {
            List<byte> enabledChannels = new List<byte>();
            foreach (ServoControl servoControl in this.servoControls)
            {
                if (this.usc != null && servoControl.servoNumber < (int)this.usc.servoCount && servoControl.mode != ChannelMode.Input)
                    enabledChannels.Add((byte)servoControl.servoNumber);
            }
            return enabledChannels;
        }
    }

    private void copyToScriptButton_Click(object sender, EventArgs e)
    {
        if (this.selectedSequence == null)
        {
            Form1.displayException("You must first create a sequence.");
        }
        else
        {
            if (this.scriptTextBox.Text != "" && !Form1.command("This will erase your current script.  Proceed?", "Copy To Script"))
                return;
            this.scriptTextBox.Text = this.selectedSequence.generateLoopedScript(this.enabledChannels);
            this.tabs.SelectedTab = this.scriptTab;
        }
    }

    private void startSequenceAtBeginning()
    {
        if (this.selectedSequence == null)
            Form1.displayException("You must first create a sequence.");
        else if (this.frameListView.Items.Count == 0)
        {
            Form1.displayException("This sequence has no frames, so it can not be played.");
        }
        else
        {
            this.currentFrame = 0;
            ushort lengthMs = this.selectedSequence.frames[this.currentFrame].length_ms;
            if (lengthMs == (ushort)0)
            {
                this.frameTimer.Interval = 1;
            }
            else
            {
                this.markAndLoadCurrentFrame();
                this.frameTimer.Interval = (int)lengthMs;
            }
            this.frameTimer.Enabled = true;
        }
    }

    protected List<Frame> selectedFrames
    {
        get
        {
            List<Frame> selectedFrames = new List<Frame>();
            Sequence selectedSequence = this.selectedSequence;
            if (selectedSequence == null)
                return selectedFrames;
            foreach (int selectedIndex in this.frameListView.SelectedIndices)
                selectedFrames.Add(selectedSequence.frames[selectedIndex]);
            return selectedFrames;
        }
    }

    private void saveCurrentServoTargetsToFrame(Frame frame)
    {
        ushort[] numArray = frame.targets = new ushort[(int)this.usc.servoCount];
        for (int index = 0; index < (int)this.usc.servoCount; ++index)
            numArray[index] = this.servoStatusControls[index].target;
    }

    private void saveFrameButton_Click(object sender, EventArgs e)
    {
        Frame frame = new Frame();
        if (this.selectedSequence == null)
            this.newSequenceButton_Click((object)null, (EventArgs)null);
        this.saveCurrentServoTargetsToFrame(frame);
        frame.name = "Frame " + (object)this.selectedSequence.frames.Count;
        frame.length_ms = this.selectedSequence.frames.Count <= 0 ? (ushort)500 : this.selectedSequence.frames[this.selectedSequence.frames.Count - 1].length_ms;
        this.selectedSequence.frames.Add(frame);
        ListViewItem listViewItem = this.frameToListViewItem(frame);
        this.frameListView.Items.Add(listViewItem);
        this.frameListView.SelectedIndices.Clear();
        this.frameListView.SelectedIndices.Add(this.frameListView.Items.Count - 1);
        this.frameListView.FocusedItem = listViewItem;
        this.refreshSaveFrameButton();
        if (this.frameListView.Visible)
            this.frameListView.Focus();
        this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
    }

    private void modifyFrameButton_Click(object sender, EventArgs e)
    {
        this.stopSequence();
        this.frameListView.Focus();
        List<Frame> selectedFrames = this.selectedFrames;
        if (selectedFrames.Count == 0)
        {
            Form1.displayException("To modify a frame, you must select it first.");
        }
        else
        {
            if (selectedFrames.Count > 1 && !Form1.confirm("Really modify " + (object)selectedFrames.Count + " frames?", "Modify Frames Confirmation"))
                return;
            foreach (Frame frame in selectedFrames)
                this.saveCurrentServoTargetsToFrame(frame);
            this.EnableApplyButton((object)null, (KeyPressEventArgs)null);
        }
    }

    private void frameListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        int num = this.suppressEvents ? 1 : 0;
    }

    private void stopSequenceButton_Click(object sender, EventArgs e) => this.stopSequence();

    private int deviceCount => this.deviceList.Items.Count - 1;

    private void OnDeviceChange()
    {
        if (this.suppressOnDeviceChange)
            return;
        try
        {
            string text = this.deviceList.Text;
            this.updateDeviceListContents();
            bool flag = this.updateDeviceListSelectedIndex();
            if (this.usc == null && this.deviceCount == 1 && !this.connectionLostLabel.Visible)
            {
                this.connectToDevice(0);
                this.updateDeviceListSelectedIndex();
                this.updateFormFromDeviceAndRegistry();
            }
            else
            {
                if (flag)
                    return;
                this.disconnect();
                this.updateFormFromDeviceAndRegistry();
                this.connectionLostLabel.Visible = true;
                this.firmwareVersionLabel.Visible = false;
                this.connectionLostLabel.Text = "Connection to " + Pololu.Usc.Usc.shortProductName + " " + text + " lost.";
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void disconnect()
    {
        if (this.usc == null)
            return;
        this.stopSequence();
        //this.usc.Dispose();
        this.usc = (Pololu.Usc.Usc)null;
    }

    private void deviceList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.suppressEvents)
            return;
        this.suppressOnDeviceChange = true;
        try
        {
            if (this.usc != null)
            {
                if (this.ApplyButton.Enabled && !Form1.confirm("The settings you changed have not been applied to the device.\nIf you disconnect from the device now, those changes will be lost.\nAre you sure you want to disconnect?", "Disconnect Confirmation"))
                {
                    this.updateDeviceListSelectedIndex();
                    return;
                }
                this.disconnect();
            }
            if (this.deviceList.SelectedIndex > 0)
                this.connectToDevice(this.deviceList.SelectedIndex - 1);
            this.updateFormFromDeviceAndRegistry();
        }
        finally
        {
            this.suppressOnDeviceChange = false;
        }
    }

    private void updateDeviceListContents()
    {
        /*
        List<DeviceListItem> connectedDevices = Pololu.Usc.Usc.getConnectedDevices();
        bool flag = false;
        if (connectedDevices.Count != this.deviceList.Items.Count - 1)
        {
            flag = true;
        }
        else
        {
            for (int index = 0; index < connectedDevices.Count; ++index)
            {
                if (!((DeviceListItem)this.deviceList.Items[index + 1]).isSameDeviceAs(connectedDevices[index]))
                {
                    flag = true;
                    break;
                }
            }
        }
        if (!flag)
            return;
        this.deviceList.Items.Clear();
        this.deviceList.DisplayMember = "text";
        this.deviceList.Items.Add((object)DeviceListItem.CreateDummyItem("Not Connected"));
        foreach (object obj in connectedDevices)
            this.deviceList.Items.Add(obj);
        */
    }

    private bool updateDeviceListSelectedIndex()
    {
        if (this.usc == null)
        {
            this.suppressEvents = true;
            this.deviceList.SelectedIndex = 0;
            this.suppressEvents = false;
            return true;
        }
        for (int index = 1; index < this.deviceList.Items.Count; ++index)
        {
            //if (this.deviceList.Items[index] is DeviceListItem deviceListItem && this.usc.isSameDeviceAs(deviceListItem))
            if (this.deviceList.Items[index] is DeviceListItem deviceListItem)
            {
                this.suppressEvents = true;
                this.deviceList.SelectedIndex = index;
                this.suppressEvents = false;
                return true;
            }
        }
        this.suppressEvents = true;
        this.deviceList.SelectedIndex = 0;
        this.suppressEvents = false;
        return false;
    }

    protected override void WndProc(ref Message m)
    {

        int msg = m.Msg;

        //detecting usb change
        /*
         if ((long)m.Msg == (long)Usb.WM_DEVICECHANGE)
         {
             try
             {
                 this.OnDeviceChange();
             }
             catch (Exception ex)
             {
                 Form1.displayException(ex, "There was an error processing a device change event.");
             }
         }
         base.WndProc(ref m);
         */
        base.WndProc(ref m);
    }

    private void connectToDevice(int index)
    {
        if (this.usc != null)
            this.disconnect();
        try
        {
            this.usc = new Pololu.Usc.Usc((DeviceListItem)this.deviceList.Items[1 + index]);
        }
        catch (Exception ex)
        {
            Form1.displayException(ex, "There was an error initializing the device.");
            return;
        }
        this.resetPerformanceFlags();
        this.connectionLostLabel.Visible = false;
        this.firmwareVersionLabel.Visible = true;
        try
        {
            this.firmwareVersionLabel.Text = "Firmware version: " + this.usc.firmwareVersionString;
        }
        catch (Exception ex)
        {
            this.firmwareVersionLabel.Text = "Firmware version unknown.";
            Form1.displayException(ex);
        }
    }

    private void backgroundUpdateDeviceList()
    {
        while (!this.windowClosed)
        {
            this.BeginInvoke((Delegate)new Form1.InvokeDelegate(this.OnDeviceChange)).AsyncWaitHandle.WaitOne();
            Thread.Sleep(1000);
        }
    }

    private void backgroundUpdateStatus()
    {
        while (!this.windowClosed)
        {
            this.BeginInvoke((Delegate)new Form1.InvokeDelegate(this.updateStatus)).AsyncWaitHandle.WaitOne();
            Thread.Sleep(50);
        }
    }

    private void updateStatus()
    {
        /*
        try
        {
            if (this.usc != null)
            {
                MaestroVariables variables;
                short[] stack;
                ushort[] callStack;
                ServoStatus[] servos;
                //this.usc.getVariables(out variables, out stack, out callStack, out servos);
                //this.errorCode.Text = "0x" + variables.errors.ToString("x4");
                if (variables.errors != (ushort)0)
                    this.errorCode.ForeColor = Color.Red;
                else
                    this.errorCode.ForeColor = Color.Black;
                this.currentErrors = variables.errors;
                for (int index = 0; index < this.performanceFlagCounts.Length; ++index)
                {
                    if (((int)variables.performanceFlags >> index & 1) == 1)
                        ++this.performanceFlagCounts[index];
                }
                if (this.tabIsSelected(this.errorsTab))
                {
                    string str1 = "";
                    foreach (uscError uscError in Enum.GetValues(typeof(uscError)))
                    {
                        if (((int)variables.errors & 1 << (int)(uscError & (uscError)31)) != 0)
                        {
                            switch (uscError)
                            {
                                case uscError.ERROR_SERIAL_SIGNAL:
                                    str1 += "Serial signal error\r\n";
                                    continue;
                                case uscError.ERROR_SERIAL_OVERRUN:
                                    str1 += "Serial overrun\r\n";
                                    continue;
                                case uscError.ERROR_SERIAL_BUFFER_FULL:
                                    str1 += "Serial buffer full\r\n";
                                    continue;
                                case uscError.ERROR_SERIAL_CRC:
                                    str1 += "Serial CRC error\r\n";
                                    continue;
                                case uscError.ERROR_SERIAL_PROTOCOL:
                                    str1 += "Serial protocol error\r\n";
                                    continue;
                                case uscError.ERROR_SERIAL_TIMEOUT:
                                    str1 += "Serial timeout\r\n";
                                    continue;
                                case uscError.ERROR_SCRIPT_STACK:
                                    str1 += "Stack overflow/underflow\r\n";
                                    continue;
                                case uscError.ERROR_SCRIPT_CALL_STACK:
                                    str1 += "Subroutine call overflow/underflow\r\n";
                                    continue;
                                case uscError.ERROR_SCRIPT_PROGRAM_COUNTER:
                                    str1 += "Script execution out of bounds\r\n";
                                    continue;
                                default:
                                    str1 = str1 + uscError.ToString() + "\r\n";
                                    continue;
                            }
                        }
                    }
                    this.errorTextBox.Text = str1;
                    string str2 = "";
                    if (this.performanceFlagCounts[0] != 0U)
                        str2 = str2 + "Advanced update late (" + (object)this.performanceFlagCounts[0] + " times)\r\n";
                    if (this.performanceFlagCounts[1] != 0U)
                        str2 = str2 + "Basic update late (" + (object)this.performanceFlagCounts[1] + " times)\r\n";
                    if (this.performanceFlagCounts[2] != 0U)
                        str2 = str2 + "Servo period exceeded (" + (object)this.performanceFlagCounts[2] + " times)\r\n";
                    this.performanceFlagsTextBox.Text = str2;
                }
                if (this.tabIsSelected(this.statusTab))
                {
                    for (int index = 0; index < (int)this.usc.servoCount; ++index)
                    {
                        this.servoStatusControls[index].target = servos[index].target;
                        this.servoStatusControls[index].position = servos[index].position;
                        this.servoStatusControls[index].speed = servos[index].speed;
                        this.servoStatusControls[index].acceleration = (ushort)servos[index].acceleration;
                    }
                }
                if (!this.tabIsSelected(this.scriptTab))
                    return;
                this.updateBytecodeProgramCounter(variables.programCounter);
                this.updateScriptButtons(variables.scriptDone == (byte)1);
                bool serious_overflow = (int)variables.stackPointer > (int)this.usc.stackSize + 1;
                this.updateStack(stack, serious_overflow);
                if ((int)variables.callStackPointer > (int)this.usc.callStackSize)
                    this.callStackLabel.Text = "Subroutines: OVERFLOW";
                else
                    this.callStackLabel.Text = "Subroutines: " + (object)callStack.Length + " of " + (object)this.usc.callStackSize + " levels used";
            }
            else
            {
                this.errorCode.ForeColor = Color.Black;
                this.errorCode.Text = "N/A";
            }
        }
        catch (Exception ex)
        {
        }

        */
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        this.windowClosed = true;
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (this.usc != null && this.ApplyButton.Enabled && !Form1.confirm("The settings you changed have not been applied to the device.\nIf you exit now, those changes will be lost.\nAre you sure you want to exit?", "Exit Confirmation"))
            return;
        this.Close();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (this.usc == null || !this.ApplyButton.Enabled || Form1.confirm("The settings you changed have not been applied to the device.\nIf you exit now, those changes will be lost.\nAre you sure you want to exit?", "Exit Confirmation"))
            return;
        e.Cancel = true;
    }

    public UscSettings getUscSettings()
    {
        UscSettings uscSettings = new UscSettings();
        //uscSettings.fixedBaudRate = (uint)this.serialFixedBaud.Value;
        uscSettings.serialTimeout = (ushort)(100M * this.serialTimeout.Value);
        uscSettings.enableCrc = this.enableCrcBox.Checked;
        uscSettings.neverSuspend = this.neverSuspendBox.Checked;
        uscSettings.serialDeviceNumber = (byte)this.serialDeviceNumber.Value;
        uscSettings.scriptDone = !this.runScriptOnStartupCheckbox.Checked;
        uscSettings.miniSscOffset = (byte)this.miniSscOffset.Value;
        if (this.usc.servoCount == (byte)6)
        {
            uscSettings.servosAvailable = (byte)this.servosAvailableUpDown.Value;
            uscSettings.servoPeriod = Pololu.Usc.Usc.microsecondsToPeriod(this.periodUpDown.Value * 1000M, uscSettings.servosAvailable);
        }
        else
        {
            uscSettings.miniMaestroServoPeriod = (uint)(this.miniMaestroServoPeriod.Value * 4000M);
            uscSettings.servoMultiplier = (ushort)this.servoMultiplier.Value;
        }
        for (byte index = 0; (int)index < (int)this.usc.servoCount; ++index)
            uscSettings.channelSettings.Add(this.servoControls[(int)index].getChannelSetting());
        //uscSettings.serialMode = !this.serialModeUsbDualPort.Checked ? (!this.serialModeUartFixed.Checked ? (!this.serialModeUsbChained.Checked ? uscSerialMode.SERIAL_MODE_UART_DETECT_BAUD_RATE : uscSerialMode.SERIAL_MODE_USB_CHAINED) : uscSerialMode.SERIAL_MODE_UART_FIXED_BAUD_RATE) : uscSerialMode.SERIAL_MODE_USB_DUAL_PORT;
        uscSettings.enablePullups = this.enablePullupsCheckBox.Checked;
        uscSettings.setAndCompileScript(this.scriptTextBox.Text);
        //uscSettings.sequences = this.sequences;
        return uscSettings;
    }

    public void setUscSettings(UscSettings settings, bool newScript)
    {
        if (this.usc.servoCount == (byte)6)
        {
            this.servosAvailableUpDown.safeSetValue((Decimal)settings.servosAvailable);
            this.updateServosOnlyAllowIO();
            try
            {
                this.periodUpDown.Value = Pololu.Usc.Usc.periodToMicroseconds((ushort)settings.servoPeriod, settings.servosAvailable) / 1000M;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                this.periodUpDown.Value = 20M;
            }
        }
        else
        {
            this.servosAvailableUpDown.safeSetValue(6M);
            this.periodUpDown.Value = 20M;
            this.updateServosOnlyAllowIO();
            try
            {
                this.miniMaestroServoPeriod.Value = (Decimal)(settings.miniMaestroServoPeriod / 4000U);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                this.miniMaestroServoPeriod.Value = 20M;
            }
            this.servoMultiplier.safeSetValue((Decimal)settings.servoMultiplier);
        }
        this.updateServoMaxLimit();
        this.updatePulseRates();
        for (byte index = 0; (int)index < (int)settings.servoCount; ++index)
            this.servoControls[(int)index].set(settings.channelSettings[(int)index]);

        //Mohan
        this.serialModeUartFixed.Checked = true;
        /*    
        switch (settings.serialMode)
        {
            case uscSerialMode.SERIAL_MODE_USB_DUAL_PORT:
                this.serialModeUsbDualPort.Checked = true;
                break;
            case uscSerialMode.SERIAL_MODE_USB_CHAINED:
                this.serialModeUsbChained.Checked = true;
                break;
            case uscSerialMode.SERIAL_MODE_UART_FIXED_BAUD_RATE:
                this.serialModeUartFixed.Checked = true;
                break;
            default:
                this.serialModeUartDetect.Checked = true;
                break;
        }
        */
        this.neverSuspendBox.Checked = settings.neverSuspend;
        this.enableCrcBox.Checked = settings.enableCrc;
        try
        {
            this.serialDeviceNumber.Value = (Decimal)settings.serialDeviceNumber;
        }
        catch (ArgumentException ex)
        {
            this.serialDeviceNumber.Value = 0M;
            Form1.displayException((Exception)ex, "The serial device number loaded from the device was out of range and has been modified.  Click Apply to save this change.");
        }
        try
        {
            this.miniSscOffset.Value = (Decimal)settings.miniSscOffset;
        }
        catch (ArgumentException ex)
        {
            this.miniSscOffset.Value = 0M;
            Form1.displayException((Exception)ex, "The mini-SSC offset loaded from the device was out of range and has been modified.  Click Apply to save this change.");
        }
        try
        {
            //this.serialFixedBaud.Value = (Decimal)settings.fixedBaudRate;
        }
        catch (ArgumentException ex)
        {
            this.serialFixedBaud.Value = 115200M;
            Form1.displayException((Exception)ex, "The baud rate loaded from the device was out of range and has been modified.  Click Apply to save this change.");
        }
        this.serialTimeout.Value = (Decimal)settings.serialTimeout / 100M;
        this.enablePullupsCheckBox.Checked = settings.enablePullups;
        this.suppressEvents = true;
        this.scriptTextBox.Text = settings.script;
        this.suppressEvents = false;
        this.runScriptOnStartupCheckbox.Checked = !settings.scriptDone;
        //this.sequences = settings.sequences;
        this.refreshSequencer();
    }

    private void restartDeviceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (this.usc == null)
            return;
        try
        {
            this.usc.reinitialize();
        }
        catch (Exception ex)
        {
            Form1.displayException(ex);
        }
    }

    private void upgradeFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (this.usc != null)
        {
            if (!Form1.confirm("This action will restart the device in bootloader mode, which\nis used for firmware upgrades.  The device will disconnect\nand reappear to your system as a new device.\nAre you sure you want to proceed?", "Firmware Upgrade"))
                return;
            try
            {
                this.usc.startBootloader();
            }
            catch (Exception ex)
            {
                Form1.displayException(ex);
            }
            try
            {
                this.disconnect();
                this.updateFormFromDeviceAndRegistry();
            }
            catch (Exception ex)
            {
                Form1.displayException(ex);
            }
        }
        //int num = (int)new Pololu.FirmwareUpgrade.FirmwareUpgrade("Maestro USB servo controller", (IList<string>)Pololu.Usc.Usc.bootloaderDeviceInstanceIdPrefixes).ShowDialog((IWin32Window)this);
    }

    private void stopScriptButton_Click(object sender, EventArgs e)
    {
        this.usc.setScriptDone((byte)1);
    }

    private bool scriptErrorMessage()
    {
        if (((int)this.currentErrors & 448) == 0)
            return false;
        Form1.warning("The script has stopped because of an error.  You must restart\nthe device to continue.", "Script Error");
        return true;
    }

    private void runScriptButton_Click(object sender, EventArgs e)
    {
        if (this.scriptErrorMessage())
            return;
        try
        {
            this.usc.setScriptDone((byte)0);
        }
        catch (Exception ex)
        {
            Form1.displayException(ex);
        }
    }

    private void stepScriptButton_Click(object sender, EventArgs e)
    {
        if (this.scriptErrorMessage())
            return;
        try
        {
            this.usc.setScriptDone((byte)2);
        }
        catch (Exception ex)
        {
            Form1.displayException(ex);
        }
    }

    private void restartScriptButton_Click(object sender, EventArgs e)
    {
        if (this.scriptErrorMessage())
            return;
        try
        {
            this.usc.restartScript();
        }
        catch (Exception ex)
        {
            Form1.displayException(ex);
        }
    }

    private void updateStack(short[] stack, bool serious_overflow)
    {
        if (this.stackListView.Items.Count != (int)this.usc.stackSize)
        {
            this.stackListView.Items.Clear();
            ListViewItem listViewItem = (ListViewItem)null;
            for (int index = (int)this.usc.stackSize - 1; index >= 0; --index)
            {
                listViewItem = new ListViewItem(new string[3]
                {
            "",
            index.ToString(),
            ""
                });
                this.stackListView.Items.Add(listViewItem);
            }
            listViewItem.EnsureVisible();
        }
        for (int index = 0; index < (int)this.usc.stackSize; ++index)
        {
            ListViewItem.ListViewSubItem subItem = this.stackListView.Items[(int)this.usc.stackSize - 1 - index].SubItems[2];
            string str = index < stack.Length ? stack[index].ToString() : "";
            if (str != subItem.Text)
                subItem.Text = str;
        }
    }

    private void updateScriptButtons(bool scriptDone)
    {
        this.restartScriptButton.Enabled = this.scriptMatchesDevice;
        if (this.scriptMatchesDevice)
            this.listingButton.Enabled = true;
        else
            this.listingButton.Enabled = false;
        if (this.usc != null && scriptDone && this.scriptMatchesDevice)
        {
            this.runScriptButton.Enabled = this.stepScriptButton.Enabled = true;
            this.runScriptButton.BackColor = Color.LimeGreen;
            this.stepScriptButton.BackColor = Color.Blue;
            this.runScriptButton.ForeColor = this.stepScriptButton.ForeColor = Color.White;
        }
        else
        {
            this.runScriptButton.Enabled = this.stepScriptButton.Enabled = false;
            this.runScriptButton.BackColor = this.stepScriptButton.BackColor = Color.Transparent;
            this.runScriptButton.ForeColor = this.stepScriptButton.ForeColor = SystemColors.ControlText;
        }
        if (this.usc != null && !scriptDone)
        {
            this.stopScriptButton.Enabled = true;
            this.stopScriptButton.BackColor = Color.Red;
            this.stopScriptButton.ForeColor = Color.White;
        }
        else
        {
            this.stopScriptButton.Enabled = false;
            this.stopScriptButton.BackColor = Color.Transparent;
            this.stopScriptButton.ForeColor = SystemColors.ControlText;
        }
    }

    private void updateBytecodeProgramCounter(ushort program_counter)
    {
        /*
        if (!this.scriptMatchesDevice || this.currentProgram == null)
        {
            this.scriptTextBox.pointerActive = false;
        }
        else
        {
            BytecodeInstruction instructionAt = this.currentProgram.getInstructionAt(program_counter);
            if (instructionAt == null)
            {
                this.scriptTextBox.pointerActive = false;
            }
            else
            {
                if (this.scrollToFollowCheckBox.Checked)
                {
                    int charIndexFromLine = this.scriptTextBox.GetFirstCharIndexFromLine(instructionAt.lineNumber - 1);
                    Rectangle rectangle = new Rectangle(this.scriptTextBox.ClientRectangle.Location, this.scriptTextBox.ClientRectangle.Size);
                    rectangle.Height -= this.scriptTextBox.Font.Height;
                    if (!rectangle.Contains(this.scriptTextBox.GetPositionFromCharIndex(charIndexFromLine)))
                    {
                        this.scriptTextBox.SelectionStart = charIndexFromLine;
                        this.scriptTextBox.ScrollToCaret();
                    }
                }
                this.scriptTextBox.pointerActive = true;
                this.scriptTextBox.pointerLine = instructionAt.lineNumber;
                this.scriptTextBox.pointerColumn = instructionAt.columnNumber;
            }
        }
        */
    }

    private void scriptTextBox_TextChanged(object sender, EventArgs e)
    {
        this.scriptTextBox.pointerActive = false;
        this.EnableApplyButton(sender, e);
        this.scriptMatchesDevice = false;
    }

    private void listingButton_Click(object sender, EventArgs e)
    {
        if (!this.scriptMatchesDevice || this.currentProgram == null)
        {
            Form1.displayException("The listing file is not available at this time because\nthe program has changed since it was last compiled.\nPlease click the Apply Settings button to compile.\n");
        }
        else
        {
            string str = Path.GetTempFileName() + ".txt";
            //BytecodeReader.WriteListing(this.currentProgram, str);
            //Process.Start(str);
        }
    }

    private void stackListView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Modifiers != Keys.Control)
            return;
        if (e.KeyCode == Keys.C)
        {
            this.copyToolStripMenuItem_Click(sender, (EventArgs)e);
        }
        else
        {
            if (e.KeyCode != Keys.A)
                return;
            foreach (ListViewItem listViewItem in this.stackListView.Items)
                listViewItem.Selected = true;
        }
    }

    public BytecodeProgram currentProgram
    {
        get
        {
            return null;
            //return this.cachedParameters != null && this.cachedParameters.bytecodeProgram != null ? this.cachedParameters.bytecodeProgram : (BytecodeProgram)null;
        }
    }

    private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (this.usc == null)
        {
            Form1.displayException("You must first connect to a device.");
        }
        else
        {
            UscSettings uscSettings;
            try
            {
                uscSettings = this.getUscSettings();
            }
            catch (Exception ex)
            {
                Form1.displayException(ex, "There was an error loading settings from the form.");
                return;
            }
            if (this.saveSettingsFileDialog.ShowDialog() != DialogResult.OK)
                return;
            Stream stream = (Stream)null;
            StreamWriter sw = (StreamWriter)null;
            try
            {
                stream = this.saveSettingsFileDialog.OpenFile();
                sw = new StreamWriter(stream);
                ConfigurationFile.save(uscSettings, sw);
            }
            catch (Exception ex)
            {
                Form1.displayException(ex, "There was an error saving the settings file.");
            }
            sw?.Close();
            stream?.Close();
        }
    }

    private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (this.usc == null)
        {
            Form1.displayException("You must first connect to a device.");
        }
        else
        {
            UscSettings settings;
            try
            {
                if (this.openSettingsFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                Stream stream = this.openSettingsFileDialog.OpenFile();
                StreamReader sr = new StreamReader(stream);
                List<string> warnings = new List<string>();
                settings = ConfigurationFile.load(sr, warnings);
                this.usc.fixSettings(settings, warnings);
                this.displaySettingWarnings(warnings, "There were problems with the settings file:", "");
                sr.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                Form1.displayException(ex, "There was an error loading the settings file.");
                return;
            }
            if (settings == null)
                return;
            try
            {
                this.setUscSettings(settings, true);
            }
            catch (Exception ex)
            {
                Form1.displayException(ex, "Error applying settings to form.");
            }
            this.EnableApplyButton(sender, e);
            this.scriptMatchesDevice = false;
        }
    }

    private void cutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Control control = this.focusedControl();
        if (control is TextBoxBase)
        {
            ((TextBoxBase)control).Cut();
        }
        else
        {
            if (control != this.frameListView)
                return;
            Frame.copyToClipboard(this.selectedFrames);
            this.removeFrames(false);
        }
    }

    private void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Control control = this.focusedControl();
        if (control is TextBoxBase)
            ((TextBoxBase)control).Copy();
        else if (control == this.stackListView)
        {
            string text = "";
            foreach (ListViewItem selectedItem in this.stackListView.SelectedItems)
                text = text + selectedItem.SubItems[1].Text.PadLeft(3) + ": " + selectedItem.SubItems[2].Text.ToString().PadLeft(6) + "\r\n";
            Clipboard.SetText(text);
        }
        else
        {
            if (control != this.frameListView)
                return;
            Frame.copyToClipboard(this.selectedFrames);
        }
    }

    private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Control control = this.focusedControl();
        if (control is TextBoxBase)
            ((TextBoxBase)control).Paste();
        if (control != this.frameListView)
            return;
        this.pasteFrames();
    }

    private Control focusedControl()
    {
        Control control1 = (Control)this;
        if (!control1.ContainsFocus)
            return (Control)null;
        label_2:
        if (control1.Controls.Count == 0)
            return control1;
        foreach (Control control2 in (ArrangedElementCollection)control1.Controls)
        {
            if (control2.ContainsFocus)
            {
                control1 = control2;
                goto label_2;
            }
        }
        return (Control)null;
    }

    private void goToOnlineDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Form1.launchDocs();
    }

    public static void launchDocs()
    {
        try
        {
            Process.Start("http://www.pololu.com/docs/0J40");
        }
        catch (Win32Exception ex)
        {
            Form1.displayException((Exception)ex);
        }
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        int num = (int)new AboutBox().ShowDialog();
    }

    private bool tabIsSelected(TabPage tab)
    {
        TabControl parent;
        try
        {
            parent = (TabControl)tab.Parent;
        }
        catch (Exception ex)
        {
            return true;
        }
        return parent.SelectedTab == tab;
    }

    private void tabs_MouseDown(object sender, MouseEventArgs e)
    {
        if (this.usc == null && AppInfo.compiledForLinux)
            return;
        for (int index = 0; index < this.tabs.TabPages.Count; ++index)
        {
            if (this.tabs.GetTabRect(index).Contains(e.Location))
            {
                this.draggingTab = this.tabs.TabPages[index];
                this.dragStartWithinTabs = e.Location;
                this.draggingTabIndex = index;
            }
        }
    }

    private void tabs_MouseUp(object sender, MouseEventArgs e)
    {
        TabPage tab = this.draggingTab;
        if (tab == null)
            return;
        if (this.tabs.GetTabRect(this.draggingTabIndex).Contains(e.Location))
        {
            this.draggingTab = null;
        }
        else
        {
            Form form = new Form();
            form.StartPosition = FormStartPosition.Manual;
            form.Text = tab.Text + " - " + this.Text;
            form.Icon = this.Icon;
            Point point = new Point(e.X - this.dragStartWithinTabs.X, e.Y - this.dragStartWithinTabs.Y);
            Point screen1 = this.tabs.PointToScreen(new Point(0, 0));
            Point screen2 = form.PointToScreen(new Point(0, 0));
            screen1.X -= screen2.X - form.Location.X;
            screen1.Y -= screen2.Y - form.Location.Y;
            screen1.X += point.X;
            screen1.Y += point.Y;
            form.Location = screen1;
            Size size = new Size();
            foreach (Control control in (ArrangedElementCollection)tab.Controls)
            {
                if (control.Right + control.Margin.Right > size.Width)
                    size.Width = control.Right + control.Margin.Right;
                if (control.Bottom + control.Margin.Bottom > size.Height)
                    size.Height = control.Bottom + control.Margin.Bottom;
            }
            size.Width += 10;
            size.Height += 10;
            form.Size = new Size(size.Width + form.Width - form.ClientRectangle.Width, size.Height + form.Height - form.ClientRectangle.Height);
            TabControl tabControl = new TabControl();
            tabControl.Size = form.ClientSize;
            tabControl.Height += this.tabs.GetTabRect(0).Height + 6;
            tabControl.Width += 4;
            tabControl.Location = new Point(-1, -this.tabs.GetTabRect(0).Height - 4);
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            form.Controls.Add((Control)tabControl);
            tabControl.TabPages.Add(tab);
            form.Show();
            form.FormClosed += (FormClosedEventHandler)((o, ee) =>
            {
                int index = 0;
                while (index < this.tabs.TabPages.Count && (int)this.tabs.TabPages[index].Tag <= (int)tab.Tag)
                    ++index;
                this.tabs.TabPages.Insert(index, tab);
                this.tabs.SelectedTab = tab;
            });
            this.draggingTab = null;
        }
    }

    private void tabs_MouseMove(object sender, MouseEventArgs e)
    {
        if (this.draggingTab == null)
            return;
        Cursor.Current = Cursors.SizeAll;
    }

    private void clearPerformanceFlagsButton_Click(object sender, EventArgs e)
    {
        this.resetPerformanceFlags();
    }

    private void resetPerformanceFlags()
    {
        for (int index = 0; index < this.performanceFlagCounts.Length; ++index)
            this.performanceFlagCounts[index] = 0U;
    }

    private void clearErrorsButton_Click(object sender, EventArgs e) => this.usc.clearErrors();

    public delegate void InvokeDelegate();
}





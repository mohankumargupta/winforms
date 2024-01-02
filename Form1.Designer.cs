namespace winforms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Pololu.MaestroControlCenter;
using Pololu.Usc;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    //private List<Sequence> sequences = new List<Sequence>();
    private System.Windows.Forms.Timer frameTimer = new System.Windows.Forms.Timer();
    private uint[] performanceFlagCounts = new uint[3];
    private Point dragStartWithinTabs = new Point();
    public const string docLink = "http://www.pololu.com/docs/0J40";
    //private ServoControl servoControl0;
    //private RoundingNumericUpDown servosAvailableUpDown;
    private ListView frameListView;
    private Button newSequenceButton;
    private Button deleteSequenceButton;
    private Button loadFrameButton;
    private Button playSequenceButton;
    private Button copyToSubroutinesButton;
    private Button copyToScriptButton;
    private Button saveFrameButton;
    private Button ApplyButton;
    //private RoundingNumericUpDown periodUpDown;
    private TabControl tabs;
    private TabPage settingsTab;
    private TabPage statusTab;
    //private ServoStatusControl servoStatusControl0;
    private Label firmwareVersionLabel;
    private Label connectionLostLabel;
    private Label label23;
    private ComboBox deviceList;
    private ComboBox sequenceList;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem saveSettingsToolStripMenuItem;
    private ToolStripMenuItem loadSettingsToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem cutToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private ToolStripMenuItem goToOnlineDocumentationToolStripMenuItem;
    private ProgressBar applyProgressBar;
    private TabPage scriptTab;
    private TabPage sequenceTab;
    private OpenFileDialog openSettingsFileDialog;
    private SaveFileDialog saveSettingsFileDialog;
    private EditorTextBox scriptTextBox;
    private LineNumberBox scriptLineNumberBox;
    private Button stepScriptButton;
    private Button runScriptButton;
    private Button stopScriptButton;
    private CheckBox scrollToFollowCheckBox;
    private Label label21;
    private Label label20;
    private CheckBox runScriptOnStartupCheckbox;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private TabPage serialSettingsTab;
    private CheckBox neverSuspendBox;
    private RadioButton serialModeUsbDualPort;
    private RadioButton serialModeUsbChained;
    private Label label25;
    private NumericUpDown serialDeviceNumber;
    private NumericUpDown serialFixedBaud;
    private RadioButton serialModeUartDetect;
    private CheckBox enableCrcBox;
    private Label serialTimeoutLabel;
    private RadioButton serialModeUartFixed;
    private RoundingNumericUpDown serialTimeout;
    private NumericUpDown miniSscOffset;
    private Label miniSSCOffsetLabel;
    private Label label27;
    private Label label9;
    private TextBox errorCode;
    private Button listingButton;
    private Button renameSequenceButton;
    private Label label22;
    private Label label24;
    private Button frameDownButton;
    private Button frameUpButton;
    private Button deleteFrameButton;
    private Button editFrameButton;
    private Button modifyFrameButton;
    private ToolTip toolTip1;
    private Button stopSequenceButton;
    private CheckBox loopBox;
    private TabPage errorsTab;
    private TextBox errorTextBox;
    private Button restartScriptButton;
    private GroupBox microMaestroAdvancedBox;
    private Button clearErrorsButton;
    private Label callStackLabel;
    private ToolStripMenuItem devicToolStripMenuItem;
    private ToolStripMenuItem restartDeviceToolStripMenuItem;
    private ToolStripMenuItem reloadSettingsToolStripMenuItem;
    private ToolStripMenuItem upgradeFirmwareToolStripMenuItem;
    private ToolStripMenuItem resetToFactorySettingsToolStripMenuItem;
    private Label scriptBytesLabel;
    private Label label30;
    private FlowLayoutPanel servoStatusFlowLayoutPanel;
    private FlowLayoutPanel servoControlFlowLayoutPanel;
    private ServoStatusControl servoStatusControl1;
    private CheckBox enablePullupsCheckBox;
    private GroupBox miniMaestroAdvancedBox;
    private RoundingNumericUpDown miniMaestroServoPeriod;
    private RoundingNumericUpDown servoMultiplier;
    private Label pulseRateLabel;
    private Button clearPerformanceFlagsButton;
    private TextBox performanceFlagsTextBox;
    private GroupBox pwmGroupBox;
    private CheckBox pwmCheckBox;
    private RoundingNumericUpDown pwmDutyCycle;
    private RoundingNumericUpDown pwmPeriod;
    private DoubleBufferedListView stackListView;
    private ColumnHeader stackIndexColumn;
    private ColumnHeader valueIndexColumn;
    private ColumnHeader stackDummyColumn;
    private Label pwmUnitsLabel;
    private ServoControl[] servoControls;
    private ServoStatusControl[] servoStatusControls;
    private Usc usc;
    private bool autoBaudWarningShown;
    private UscSettings cachedParameters;
    private int currentFrame;
    //private Sequence selectedSequence;
    private bool suppressOnDeviceChange;
    private bool suppressEvents;
    private bool windowClosed;
    private ushort currentErrors;
    private bool scriptMatchesDevice;
    private TabPage draggingTab;
    private int draggingTabIndex;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Form1";
    }

    #endregion
}

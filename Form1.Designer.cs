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
using Pololu.Usc.Sequencer;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    //private List<Sequence> sequences = new List<Sequence>();
    private System.Windows.Forms.Timer frameTimer = new System.Windows.Forms.Timer();
    private uint[] performanceFlagCounts = new uint[3];

    public const string docLink = "http://www.pololu.com/docs/0J40";

    private ServoControl servoControl0;
    private RoundingNumericUpDown servosAvailableUpDown;
    private ListView frameListView;
    private Button newSequenceButton;
    private Button deleteSequenceButton;
    private Button loadFrameButton;
    private Button playSequenceButton;
    private Button copyToSubroutinesButton;
    private Button copyToScriptButton;
    private Button saveFrameButton;
    private Button ApplyButton;
    private RoundingNumericUpDown periodUpDown;
    private TabControl tabs;
    private TabPage settingsTab;
    private TabPage statusTab;
    private ServoStatusControl servoStatusControl0;
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
    private Pololu.Usc.Usc usc;
    private bool autoBaudWarningShown;
    private UscSettings cachedParameters;
    private List<Sequence> sequences = new List<Sequence>();
    //private System.Windows.Forms.Timer frameTimer = new System.Windows.Forms.Timer();
    private int currentFrame;
    private Sequence selectedSequence;
    private bool suppressOnDeviceChange;
    private bool suppressEvents;
    private bool windowClosed;
    private ushort currentErrors;
    //private uint[] performanceFlagCounts = new uint[3];
    private bool scriptMatchesDevice;
    private TabPage draggingTab;
    private Point dragStartWithinTabs = new Point();
    private int draggingTabIndex;

    /*
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
    */

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
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Form1";

        this.pwmUnitsLabel = new Label();
        this.frameListView = new ListView();
        this.columnHeader1 = new ColumnHeader();
        this.columnHeader2 = new ColumnHeader();
        this.columnHeader3 = new ColumnHeader();
        this.newSequenceButton = new Button();
        this.deleteSequenceButton = new Button();
        this.loadFrameButton = new Button();
        this.playSequenceButton = new Button();
        this.copyToScriptButton = new Button();
        this.copyToSubroutinesButton = new Button();
        this.saveFrameButton = new Button();
        this.ApplyButton = new Button();
        this.tabs = new TabControl();
        this.statusTab = new TabPage();
        this.pwmGroupBox = new GroupBox();
        this.pwmCheckBox = new CheckBox();
        this.servoStatusFlowLayoutPanel = new FlowLayoutPanel();
        this.errorsTab = new TabPage();
        this.performanceFlagsTextBox = new TextBox();
        this.clearPerformanceFlagsButton = new Button();
        this.clearErrorsButton = new Button();
        this.errorTextBox = new TextBox();
        this.settingsTab = new TabPage();
        this.pulseRateLabel = new Label();
        this.miniMaestroAdvancedBox = new GroupBox();
        this.enablePullupsCheckBox = new CheckBox();
        this.servoControlFlowLayoutPanel = new FlowLayoutPanel();
        this.microMaestroAdvancedBox = new GroupBox();
        this.serialSettingsTab = new TabPage();
        this.label27 = new Label();
        this.miniSSCOffsetLabel = new Label();
        this.miniSscOffset = new NumericUpDown();
        this.neverSuspendBox = new CheckBox();
        this.serialModeUsbDualPort = new RadioButton();
        this.serialModeUsbChained = new RadioButton();
        this.label25 = new Label();
        this.serialDeviceNumber = new NumericUpDown();
        this.serialFixedBaud = new NumericUpDown();
        this.serialModeUartDetect = new RadioButton();
        this.enableCrcBox = new CheckBox();
        this.serialTimeoutLabel = new Label();
        this.serialModeUartFixed = new RadioButton();
        this.sequenceTab = new TabPage();
        this.label30 = new Label();
        this.loopBox = new CheckBox();
        this.stopSequenceButton = new Button();
        this.modifyFrameButton = new Button();
        this.frameDownButton = new Button();
        this.frameUpButton = new Button();
        this.deleteFrameButton = new Button();
        this.editFrameButton = new Button();
        this.label24 = new Label();
        this.renameSequenceButton = new Button();
        this.label22 = new Label();
        this.sequenceList = new ComboBox();
        this.scriptTab = new TabPage();
        this.scriptBytesLabel = new Label();
        this.callStackLabel = new Label();
        this.restartScriptButton = new Button();
        this.listingButton = new Button();
        this.runScriptOnStartupCheckbox = new CheckBox();
        this.label21 = new Label();
        this.label20 = new Label();
        this.scrollToFollowCheckBox = new CheckBox();
        this.stepScriptButton = new Button();
        this.runScriptButton = new Button();
        this.stopScriptButton = new Button();
        this.applyProgressBar = new ProgressBar();
        this.firmwareVersionLabel = new Label();
        this.connectionLostLabel = new Label();
        this.label23 = new Label();
        this.deviceList = new ComboBox();
        this.menuStrip1 = new MenuStrip();
        this.fileToolStripMenuItem = new ToolStripMenuItem();
        this.saveSettingsToolStripMenuItem = new ToolStripMenuItem();
        this.loadSettingsToolStripMenuItem = new ToolStripMenuItem();
        this.exitToolStripMenuItem = new ToolStripMenuItem();
        this.devicToolStripMenuItem = new ToolStripMenuItem();
        this.restartDeviceToolStripMenuItem = new ToolStripMenuItem();
        this.reloadSettingsToolStripMenuItem = new ToolStripMenuItem();
        this.resetToFactorySettingsToolStripMenuItem = new ToolStripMenuItem();
        this.upgradeFirmwareToolStripMenuItem = new ToolStripMenuItem();
        this.editToolStripMenuItem = new ToolStripMenuItem();
        this.cutToolStripMenuItem = new ToolStripMenuItem();
        this.copyToolStripMenuItem = new ToolStripMenuItem();
        this.pasteToolStripMenuItem = new ToolStripMenuItem();
        this.helpToolStripMenuItem = new ToolStripMenuItem();
        this.aboutToolStripMenuItem = new ToolStripMenuItem();
        this.goToOnlineDocumentationToolStripMenuItem = new ToolStripMenuItem();
        this.openSettingsFileDialog = new OpenFileDialog();
        this.saveSettingsFileDialog = new SaveFileDialog();
        this.label9 = new Label();
        this.errorCode = new TextBox();
        this.toolTip1 = new ToolTip(this.components);
        this.pwmPeriod = new RoundingNumericUpDown();
        this.pwmDutyCycle = new RoundingNumericUpDown();
        this.servoStatusControl0 = new ServoStatusControl();
        this.servoMultiplier = new RoundingNumericUpDown();
        this.miniMaestroServoPeriod = new RoundingNumericUpDown();
        this.servoControl0 = new ServoControl();
        this.periodUpDown = new RoundingNumericUpDown();
        this.servosAvailableUpDown = new RoundingNumericUpDown();
        this.serialTimeout = new RoundingNumericUpDown();
        this.stackListView = new DoubleBufferedListView();
        this.stackDummyColumn = new ColumnHeader();
        this.stackIndexColumn = new ColumnHeader();
        this.valueIndexColumn = new ColumnHeader();
        this.scriptLineNumberBox = new LineNumberBox();
        this.scriptTextBox = new EditorTextBox();
        this.servoStatusControl1 = new ServoStatusControl();
        Label label1 = new Label();
        Label label2 = new Label();
        Label label3 = new Label();
        Label label4 = new Label();
        Label label5 = new Label();
        Label label6 = new Label();
        Label label7 = new Label();
        Label label8 = new Label();
        Label label9 = new Label();
        Label label10 = new Label();
        Label label11 = new Label();
        Label label12 = new Label();
        Label label13 = new Label();
        Label label14 = new Label();
        Label label15 = new Label();
        Label label16 = new Label();
        Label label17 = new Label();
        Label label18 = new Label();
        Label label19 = new Label();
        Label label20 = new Label();
        Label label21 = new Label();
        Label label22 = new Label();
        Label label23 = new Label();
        Label label24 = new Label();
        Label label25 = new Label();
        Label label26 = new Label();
        this.tabs.SuspendLayout();
        this.statusTab.SuspendLayout();
        this.pwmGroupBox.SuspendLayout();
        this.servoStatusFlowLayoutPanel.SuspendLayout();
        this.errorsTab.SuspendLayout();
        this.settingsTab.SuspendLayout();
        this.miniMaestroAdvancedBox.SuspendLayout();
        this.servoControlFlowLayoutPanel.SuspendLayout();
        this.microMaestroAdvancedBox.SuspendLayout();
        this.serialSettingsTab.SuspendLayout();
        this.miniSscOffset.BeginInit();
        this.serialDeviceNumber.BeginInit();
        this.serialFixedBaud.BeginInit();
        this.sequenceTab.SuspendLayout();
        this.scriptTab.SuspendLayout();
        this.menuStrip1.SuspendLayout();
        this.pwmPeriod.BeginInit();
        this.pwmDutyCycle.BeginInit();
        this.servoMultiplier.BeginInit();
        this.miniMaestroServoPeriod.BeginInit();
        this.periodUpDown.BeginInit();
        this.servosAvailableUpDown.BeginInit();
        this.serialTimeout.BeginInit();
        this.SuspendLayout();
        label1.AutoSize = true;
        label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label1.Location = new Point(135, 4);
        label1.Name = "label1";
        label1.Size = new Size(34, 13);
        label1.TabIndex = 2;
        label1.Text = "Mode";
        label2.AutoSize = true;
        label2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label2.Location = new Point(258, 4);
        label2.Name = "label2";
        label2.Size = new Size(24, 13);
        label2.TabIndex = 3;
        label2.Text = "Min";
        label3.AutoSize = true;
        label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label3.Location = new Point(320, 4);
        label3.Name = "label3";
        label3.Size = new Size(27, 13);
        label3.TabIndex = 4;
        label3.Text = "Max";
        label4.AutoSize = true;
        label4.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label4.Location = new Point(720, 4);
        label4.Name = "label4";
        label4.Size = new Size(80, 13);
        label4.TabIndex = 9;
        label4.Text = "8-bit range (+/-)";
        label5.AutoSize = true;
        label5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label5.Location = new Point(647, 4);
        label5.Name = "label5";
        label5.Size = new Size(62, 13);
        label5.TabIndex = 8;
        label5.Text = "8-bit neutral";
        label6.AutoSize = true;
        label6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label6.Location = new Point(374, 4);
        label6.Name = "label6";
        label6.Size = new Size(95, 13);
        label6.TabIndex = 5;
        label6.Text = "On startup or error:";
        label7.AutoSize = true;
        label7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label7.Location = new Point(519, 4);
        label7.Name = "label7";
        label7.Size = new Size(38, 13);
        label7.TabIndex = 6;
        label7.Text = "Speed";
        label8.AutoSize = true;
        label8.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label8.Location = new Point(574, 4);
        label8.Name = "label8";
        label8.Size = new Size(66, 13);
        label8.TabIndex = 7;
        label8.Text = "Acceleration";
        label9.AutoSize = true;
        label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label9.Location = new Point(28, 32);
        label9.Name = "label10";
        label9.Size = new Size(88, 13);
        label9.TabIndex = 0;
        label9.Text = "Servos available:";
        label10.AutoSize = true;
        label10.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label10.Location = new Point(116, 4);
        label10.Name = "label29";
        label10.Size = new Size(34, 13);
        label10.TabIndex = 2;
        label10.Text = "Mode";
        label11.AutoSize = true;
        label11.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label11.Location = new Point(13, 4);
        label11.Name = "label17";
        label11.Size = new Size(14, 13);
        label11.TabIndex = 0;
        label11.Text = "#";
        label12.AutoSize = true;
        label12.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label12.Location = new Point(32, 4);
        label12.Name = "label16";
        label12.Size = new Size(35, 13);
        label12.TabIndex = 1;
        label12.Text = "Name";
        label13.AutoSize = true;
        label13.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label13.Location = new Point(157, 4);
        label13.Name = "label15";
        label13.Size = new Size(46, 13);
        label13.TabIndex = 3;
        label13.Text = "Enabled";
        label14.AutoSize = true;
        label14.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label14.Location = new Point(658, 4);
        label14.Name = "label14";
        label14.Size = new Size(44, 13);
        label14.TabIndex = 7;
        label14.Text = "Position";
        label15.AutoSize = true;
        label15.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label15.Location = new Point(434, 4);
        label15.Name = "label13";
        label15.Size = new Size(38, 13);
        label15.TabIndex = 4;
        label15.Text = "Target";
        label16.AutoSize = true;
        label16.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label16.Location = new Point(573, 4);
        label16.Name = "label11";
        label16.Size = new Size(66, 13);
        label16.TabIndex = 6;
        label16.Text = "Acceleration";
        label17.AutoSize = true;
        label17.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label17.Location = new Point(508, 4);
        label17.Name = "label12";
        label17.Size = new Size(38, 13);
        label17.TabIndex = 5;
        label17.Text = "Speed";
        label18.AutoSize = true;
        label18.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label18.Location = new Point(28, 58);
        label18.Name = "label26";
        label18.Size = new Size(62, 13);
        label18.TabIndex = 2;
        label18.Text = "Period (ms):";
        label19.AutoSize = true;
        label19.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label19.Location = new Point(32, 4);
        label19.Name = "label19";
        label19.Size = new Size(35, 13);
        label19.TabIndex = 1;
        label19.Text = "Name";
        label20.AutoSize = true;
        label20.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label20.Location = new Point(13, 4);
        label20.Name = "label18";
        label20.Size = new Size(14, 13);
        label20.TabIndex = 0;
        label20.Text = "#";
        label21.AutoSize = true;
        label21.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label21.Location = new Point(28, 32);
        label21.Name = "label31";
        label21.Size = new Size(62, 13);
        label21.TabIndex = 2;
        label21.Text = "Period (ms):";
        label22.AutoSize = true;
        label22.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label22.Location = new Point(28, 63);
        label22.Name = "label32";
        label22.Size = new Size(83, 13);
        label22.TabIndex = 4;
        label22.Text = "Period multiplier:";
        label23.AutoSize = true;
        label23.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label23.Location = new Point(15, 51);
        label23.Name = "label33";
        label23.Size = new Size(46, 13);
        label23.TabIndex = 4;
        label23.Text = "On time:";
        label24.AutoSize = true;
        label24.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        label24.Location = new Point(15, 77);
        label24.Name = "label34";
        label24.Size = new Size(40, 13);
        label24.TabIndex = 21;
        label24.Text = "Period:";
        label25.AutoSize = true;
        label25.ForeColor = SystemColors.ControlText;
        label25.Location = new Point(6, 3);
        label25.Name = "label28";
        label25.Size = new Size(112, 13);
        label25.TabIndex = 0;
        label25.Text = "Currently active errors:";
        label26.AutoSize = true;
        label26.ForeColor = SystemColors.ControlText;
        label26.Location = new Point(275, 3);
        label26.Name = "label36";
        label26.Size = new Size(95, 13);
        label26.TabIndex = 7;
        label26.Text = "Performance flags:";
        this.pwmUnitsLabel.AutoSize = true;
        this.pwmUnitsLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.pwmUnitsLabel.Location = new Point(146, 62);
        this.pwmUnitsLabel.Name = "pwmUnitsLabel";
        this.pwmUnitsLabel.Size = new Size(74, 26);
        this.pwmUnitsLabel.TabIndex = 22;
        this.pwmUnitsLabel.Text = "Units: 1/48 μs\r\n(20.8 ns)";
        this.pwmUnitsLabel.TextAlign = ContentAlignment.MiddleCenter;
        this.frameListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        this.frameListView.Columns.AddRange(new ColumnHeader[3]
        {
        this.columnHeader1,
        this.columnHeader2,
        this.columnHeader3
        });
        this.frameListView.FullRowSelect = true;
        this.frameListView.LabelWrap = false;
        this.frameListView.Location = new Point(10, 65);
        this.frameListView.Name = "frameListView";
        this.frameListView.Size = new Size(272, 362);
        this.frameListView.TabIndex = 7;
        this.frameListView.UseCompatibleStateImageBehavior = false;
        this.frameListView.View = View.Details;
        this.frameListView.SelectedIndexChanged += new EventHandler(this.frameListView_SelectedIndexChanged);
        this.frameListView.DoubleClick += new EventHandler(this.frameListView_DoubleClick);
        this.frameListView.KeyDown += new KeyEventHandler(this.frameListView_KeyDown);
        this.columnHeader1.Text = "";
        this.columnHeader1.Width = 22;
        this.columnHeader2.Text = "Frame name";
        this.columnHeader2.Width = 167;
        this.columnHeader3.Text = "Duration (ms)";
        this.columnHeader3.Width = 76;
        this.newSequenceButton.Location = new Point(419, 20);
        this.newSequenceButton.Name = "newSequenceButton";
        this.newSequenceButton.Size = new Size(170, 23);
        this.newSequenceButton.TabIndex = 4;
        this.newSequenceButton.Text = "Ne&w Sequence";
        this.newSequenceButton.UseVisualStyleBackColor = true;
        this.newSequenceButton.Click += new EventHandler(this.newSequenceButton_Click);
        this.deleteSequenceButton.Location = new Point(346, 20);
        this.deleteSequenceButton.Name = "deleteSequenceButton";
        this.deleteSequenceButton.Size = new Size(50, 23);
        this.deleteSequenceButton.TabIndex = 3;
        this.deleteSequenceButton.Text = "Dele&te";
        this.deleteSequenceButton.UseVisualStyleBackColor = true;
        this.deleteSequenceButton.Click += new EventHandler(this.deleteSequenceButton_Click);
        this.loadFrameButton.Location = new Point(288, 183);
        this.loadFrameButton.Name = "loadFrameButton";
        this.loadFrameButton.Size = new Size(119, 23);
        this.loadFrameButton.TabIndex = 13;
        this.loadFrameButton.Text = "&Load Frame";
        this.toolTip1.SetToolTip((Control)this.loadFrameButton, "Sets the targets of all the servos according to the current frame.");
        this.loadFrameButton.UseVisualStyleBackColor = true;
        this.loadFrameButton.Click += new EventHandler(this.loadFrameButton_Click);
        this.playSequenceButton.Location = new Point(288, 93);
        this.playSequenceButton.Name = "playSequenceButton";
        this.playSequenceButton.Size = new Size(119, 23);
        this.playSequenceButton.TabIndex = 10;
        this.playSequenceButton.Text = "&Play Sequence";
        this.toolTip1.SetToolTip((Control)this.playSequenceButton, "Plays back the sequence under computer control.");
        this.playSequenceButton.UseVisualStyleBackColor = true;
        this.playSequenceButton.Click += new EventHandler(this.playSequenceButton_Click);
        this.copyToScriptButton.Location = new Point(419, 93);
        this.copyToScriptButton.Name = "copyToScriptButton";
        this.copyToScriptButton.Size = new Size(170, 23);
        this.copyToScriptButton.TabIndex = 11;
        this.copyToScriptButton.Text = "C&opy Sequence to Script";
        this.toolTip1.SetToolTip((Control)this.copyToScriptButton, "Creates a representation of the sequence as a script.\r\nRunning the script will causes the device to play back\r\nthe sequence in a loop.");
        this.copyToScriptButton.UseVisualStyleBackColor = true;
        this.copyToScriptButton.Click += new EventHandler(this.copyToScriptButton_Click);
        this.copyToSubroutinesButton.Location = new Point(419, 49);
        this.copyToSubroutinesButton.Name = "copyToSubroutinesButton";
        this.copyToSubroutinesButton.Size = new Size(170, 23);
        this.copyToSubroutinesButton.TabIndex = 5;
        this.copyToSubroutinesButton.Text = "&Copy all Sequences to Script";
        this.toolTip1.SetToolTip((Control)this.copyToSubroutinesButton, "Copies all of the sequences to separate subroutines at the\r\nend of the script.  The intent is for you to call these\r\nsubroutines from your own custom code.");
        this.copyToSubroutinesButton.UseVisualStyleBackColor = true;
        this.copyToSubroutinesButton.Click += new EventHandler(this.copyToSubroutinesButton_Click);
        this.saveFrameButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.saveFrameButton.Location = new Point(12, 492);
        this.saveFrameButton.Name = "saveFrameButton";
        this.saveFrameButton.Size = new Size(160, 23);
        this.saveFrameButton.TabIndex = 6;
        this.saveFrameButton.Text = "&Save Frame";
        this.toolTip1.SetToolTip((Control)this.saveFrameButton, "Saves the current servo positions as a new frame in the Sequence tab.");
        this.saveFrameButton.UseVisualStyleBackColor = true;
        this.saveFrameButton.Click += new EventHandler(this.saveFrameButton_Click);
        this.ApplyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.ApplyButton.Location = new Point(663, 492);
        this.ApplyButton.Name = "ApplyButton";
        this.ApplyButton.Size = new Size(148, 23);
        this.ApplyButton.TabIndex = 8;
        this.ApplyButton.Text = "&Apply Settings";
        this.toolTip1.SetToolTip((Control)this.ApplyButton, "Programs all settings and the script onto the device.");
        this.ApplyButton.UseVisualStyleBackColor = true;
        this.ApplyButton.Click += new EventHandler(this.ApplyButton_Click);
        this.tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.tabs.Controls.Add((Control)this.statusTab);
        this.tabs.Controls.Add((Control)this.errorsTab);
        this.tabs.Controls.Add((Control)this.settingsTab);
        this.tabs.Controls.Add((Control)this.serialSettingsTab);
        this.tabs.Controls.Add((Control)this.sequenceTab);
        this.tabs.Controls.Add((Control)this.scriptTab);
        this.tabs.Location = new Point(0, 58);
        this.tabs.Name = "tabs";
        this.tabs.SelectedIndex = 0;
        this.tabs.Size = new Size(823, 432);
        this.tabs.TabIndex = 5;
        this.tabs.MouseMove += new MouseEventHandler(this.tabs_MouseMove);
        this.tabs.MouseDown += new MouseEventHandler(this.tabs_MouseDown);
        this.tabs.MouseUp += new MouseEventHandler(this.tabs_MouseUp);
        this.statusTab.AutoScroll = true;
        this.statusTab.BackColor = Color.Transparent;
        this.statusTab.Controls.Add((Control)this.pwmGroupBox);
        this.statusTab.Controls.Add((Control)this.servoStatusFlowLayoutPanel);
        this.statusTab.Controls.Add((Control)label10);
        this.statusTab.Controls.Add((Control)label11);
        this.statusTab.Controls.Add((Control)label12);
        this.statusTab.Controls.Add((Control)label13);
        this.statusTab.Controls.Add((Control)label14);
        this.statusTab.Controls.Add((Control)label15);
        this.statusTab.Controls.Add((Control)label16);
        this.statusTab.Controls.Add((Control)label17);
        this.statusTab.Location = new Point(4, 22);
        this.statusTab.Name = "statusTab";
        this.statusTab.Padding = new Padding(3);
        this.statusTab.Size = new Size(815, 406);
        this.statusTab.TabIndex = 1;
        this.statusTab.Tag = (object)"0";
        this.statusTab.Text = "Status";
        this.statusTab.UseVisualStyleBackColor = true;
        this.pwmGroupBox.Controls.Add((Control)this.pwmUnitsLabel);
        this.pwmGroupBox.Controls.Add((Control)label24);
        this.pwmGroupBox.Controls.Add((Control)this.pwmPeriod);
        this.pwmGroupBox.Controls.Add((Control)this.pwmCheckBox);
        this.pwmGroupBox.Controls.Add((Control)this.pwmDutyCycle);
        this.pwmGroupBox.Controls.Add((Control)label23);
        this.pwmGroupBox.Location = new Point(8, 278);
        this.pwmGroupBox.Margin = new Padding(3, 20, 3, 3);
        this.pwmGroupBox.Name = "pwmGroupBox";
        this.pwmGroupBox.Size = new Size(231, 116);
        this.pwmGroupBox.TabIndex = 24;
        this.pwmGroupBox.TabStop = false;
        this.pwmGroupBox.Text = "PWM Output";
        this.pwmCheckBox.AutoSize = true;
        this.pwmCheckBox.Location = new Point(18, 26);
        this.pwmCheckBox.Name = "pwmCheckBox";
        this.pwmCheckBox.Size = new Size(154, 17);
        this.pwmCheckBox.TabIndex = 0;
        this.pwmCheckBox.Text = "Enable PWM on channel 8";
        this.pwmCheckBox.UseVisualStyleBackColor = true;
        this.pwmCheckBox.CheckedChanged += new EventHandler(this.updatePWM);
        this.servoStatusFlowLayoutPanel.Controls.Add((Control)this.servoStatusControl0);
        this.servoStatusFlowLayoutPanel.FlowDirection = FlowDirection.TopDown;
        this.servoStatusFlowLayoutPanel.Location = new Point(0, 20);
        this.servoStatusFlowLayoutPanel.MaximumSize = new Size(724, 1400);
        this.servoStatusFlowLayoutPanel.Name = "servoStatusFlowLayoutPanel";
        this.servoStatusFlowLayoutPanel.Size = new Size(724, 137);
        this.servoStatusFlowLayoutPanel.TabIndex = 9;
        this.servoStatusFlowLayoutPanel.WrapContents = false;
        this.errorsTab.Controls.Add((Control)this.performanceFlagsTextBox);
        this.errorsTab.Controls.Add((Control)this.clearPerformanceFlagsButton);
        this.errorsTab.Controls.Add((Control)label26);
        this.errorsTab.Controls.Add((Control)this.clearErrorsButton);
        this.errorsTab.Controls.Add((Control)label25);
        this.errorsTab.Controls.Add((Control)this.errorTextBox);
        this.errorsTab.Location = new Point(4, 22);
        this.errorsTab.Name = "errorsTab";
        this.errorsTab.Padding = new Padding(3);
        this.errorsTab.Size = new Size(815, 446);
        this.errorsTab.TabIndex = 5;
        this.errorsTab.Tag = (object)"1";
        this.errorsTab.Text = "Errors";
        this.errorsTab.UseVisualStyleBackColor = true;
        this.performanceFlagsTextBox.Location = new Point(278, 19);
        this.performanceFlagsTextBox.Multiline = true;
        this.performanceFlagsTextBox.Name = "performanceFlagsTextBox";
        this.performanceFlagsTextBox.ReadOnly = true;
        this.performanceFlagsTextBox.Size = new Size(215, 98);
        this.performanceFlagsTextBox.TabIndex = 3;
        this.clearPerformanceFlagsButton.Location = new Point(278, 123);
        this.clearPerformanceFlagsButton.Name = "clearPerformanceFlagsButton";
        this.clearPerformanceFlagsButton.Size = new Size(154, 23);
        this.clearPerformanceFlagsButton.TabIndex = 5;
        this.clearPerformanceFlagsButton.Text = "Clear Performance Flags";
        this.clearPerformanceFlagsButton.UseVisualStyleBackColor = true;
        this.clearPerformanceFlagsButton.Click += new EventHandler(this.clearPerformanceFlagsButton_Click);
        this.clearErrorsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.clearErrorsButton.Location = new Point(6, 417);
        this.clearErrorsButton.Name = "clearErrorsButton";
        this.clearErrorsButton.Size = new Size(108, 23);
        this.clearErrorsButton.TabIndex = 2;
        this.clearErrorsButton.Text = "&Clear Errors";
        this.clearErrorsButton.UseVisualStyleBackColor = true;
        this.clearErrorsButton.Click += new EventHandler(this.clearErrorsButton_Click);
        this.errorTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        this.errorTextBox.Location = new Point(6, 19);
        this.errorTextBox.Multiline = true;
        this.errorTextBox.Name = "errorTextBox";
        this.errorTextBox.ReadOnly = true;
        this.errorTextBox.Size = new Size(230, 392);
        this.errorTextBox.TabIndex = 1;
        this.settingsTab.AutoScroll = true;
        this.settingsTab.BackColor = Color.Transparent;
        this.settingsTab.Controls.Add((Control)this.pulseRateLabel);
        this.settingsTab.Controls.Add((Control)this.miniMaestroAdvancedBox);
        this.settingsTab.Controls.Add((Control)this.enablePullupsCheckBox);
        this.settingsTab.Controls.Add((Control)this.servoControlFlowLayoutPanel);
        this.settingsTab.Controls.Add((Control)this.microMaestroAdvancedBox);
        this.settingsTab.Controls.Add((Control)label19);
        this.settingsTab.Controls.Add((Control)label20);
        this.settingsTab.Controls.Add((Control)label1);
        this.settingsTab.Controls.Add((Control)label8);
        this.settingsTab.Controls.Add((Control)label2);
        this.settingsTab.Controls.Add((Control)label7);
        this.settingsTab.Controls.Add((Control)label3);
        this.settingsTab.Controls.Add((Control)label4);
        this.settingsTab.Controls.Add((Control)label6);
        this.settingsTab.Controls.Add((Control)label5);
        this.settingsTab.Location = new Point(4, 22);
        this.settingsTab.Name = "settingsTab";
        this.settingsTab.Padding = new Padding(3);
        this.settingsTab.Size = new Size(815, 446);
        this.settingsTab.TabIndex = 0;
        this.settingsTab.Tag = (object)"2";
        this.settingsTab.Text = "Channel Settings";
        this.settingsTab.UseVisualStyleBackColor = true;
        this.pulseRateLabel.AutoSize = true;
        this.pulseRateLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.pulseRateLabel.Location = new Point(186, 4);
        this.pulseRateLabel.Name = "pulseRateLabel";
        this.pulseRateLabel.Size = new Size(52, 13);
        this.pulseRateLabel.TabIndex = 19;
        this.pulseRateLabel.Text = "Rate (Hz)";
        this.miniMaestroAdvancedBox.Controls.Add((Control)this.servoMultiplier);
        this.miniMaestroAdvancedBox.Controls.Add((Control)label22);
        this.miniMaestroAdvancedBox.Controls.Add((Control)label21);
        this.miniMaestroAdvancedBox.Controls.Add((Control)this.miniMaestroServoPeriod);
        this.miniMaestroAdvancedBox.Location = new Point(453, 250);
        this.miniMaestroAdvancedBox.Margin = new Padding(3, 20, 3, 3);
        this.miniMaestroAdvancedBox.Name = "miniMaestroAdvancedBox";
        this.miniMaestroAdvancedBox.Size = new Size(200, 100);
        this.miniMaestroAdvancedBox.TabIndex = 26;
        this.miniMaestroAdvancedBox.TabStop = false;
        this.miniMaestroAdvancedBox.Text = "Advanced Pulse Control";
        this.enablePullupsCheckBox.AutoSize = true;
        this.enablePullupsCheckBox.Location = new Point(300, 383);
        this.enablePullupsCheckBox.Name = "enablePullupsCheckBox";
        this.enablePullupsCheckBox.Size = new Size(189, 17);
        this.enablePullupsCheckBox.TabIndex = 24;
        this.enablePullupsCheckBox.Text = "Enable pull-ups on channels 18-20";
        this.toolTip1.SetToolTip((Control)this.enablePullupsCheckBox, "Enables a pull-up resistor for each channel 18-20 which is configured as an input.  This makes the input go high by default.");
        this.enablePullupsCheckBox.UseVisualStyleBackColor = true;
        this.enablePullupsCheckBox.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.servoControlFlowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.servoControlFlowLayoutPanel.Controls.Add((Control)this.servoControl0);
        this.servoControlFlowLayoutPanel.FlowDirection = FlowDirection.TopDown;
        this.servoControlFlowLayoutPanel.Location = new Point(0, 20);
        this.servoControlFlowLayoutPanel.MaximumSize = new Size(800, 1400);
        this.servoControlFlowLayoutPanel.Name = "servoControlFlowLayoutPanel";
        this.servoControlFlowLayoutPanel.Size = new Size(800, 179);
        this.servoControlFlowLayoutPanel.TabIndex = 17;
        this.microMaestroAdvancedBox.Controls.Add((Control)label18);
        this.microMaestroAdvancedBox.Controls.Add((Control)this.periodUpDown);
        this.microMaestroAdvancedBox.Controls.Add((Control)this.servosAvailableUpDown);
        this.microMaestroAdvancedBox.Controls.Add((Control)label9);
        this.microMaestroAdvancedBox.Location = new Point(240, 250);
        this.microMaestroAdvancedBox.Margin = new Padding(3, 20, 3, 3);
        this.microMaestroAdvancedBox.Name = "microMaestroAdvancedBox";
        this.microMaestroAdvancedBox.Size = new Size(200, 100);
        this.microMaestroAdvancedBox.TabIndex = 25;
        this.microMaestroAdvancedBox.TabStop = false;
        this.microMaestroAdvancedBox.Text = "Advanced Pulse Control";
        this.serialSettingsTab.Controls.Add((Control)this.label27);
        this.serialSettingsTab.Controls.Add((Control)this.miniSSCOffsetLabel);
        this.serialSettingsTab.Controls.Add((Control)this.miniSscOffset);
        this.serialSettingsTab.Controls.Add((Control)this.neverSuspendBox);
        this.serialSettingsTab.Controls.Add((Control)this.serialModeUsbDualPort);
        this.serialSettingsTab.Controls.Add((Control)this.serialModeUsbChained);
        this.serialSettingsTab.Controls.Add((Control)this.label25);
        this.serialSettingsTab.Controls.Add((Control)this.serialDeviceNumber);
        this.serialSettingsTab.Controls.Add((Control)this.serialFixedBaud);
        this.serialSettingsTab.Controls.Add((Control)this.serialModeUartDetect);
        this.serialSettingsTab.Controls.Add((Control)this.enableCrcBox);
        this.serialSettingsTab.Controls.Add((Control)this.serialTimeoutLabel);
        this.serialSettingsTab.Controls.Add((Control)this.serialModeUartFixed);
        this.serialSettingsTab.Controls.Add((Control)this.serialTimeout);
        this.serialSettingsTab.Location = new Point(4, 22);
        this.serialSettingsTab.Name = "serialSettingsTab";
        this.serialSettingsTab.Padding = new Padding(3);
        this.serialSettingsTab.Size = new Size(815, 446);
        this.serialSettingsTab.TabIndex = 4;
        this.serialSettingsTab.Tag = (object)"3";
        this.serialSettingsTab.Text = "Serial Settings";
        this.serialSettingsTab.UseVisualStyleBackColor = true;
        this.label27.AutoSize = true;
        this.label27.Location = new Point(3, 9);
        this.label27.Name = "label27";
        this.label27.Size = new Size(65, 13);
        this.label27.TabIndex = 0;
        this.label27.Text = "Serial mode:";
        this.miniSSCOffsetLabel.AutoSize = true;
        this.miniSSCOffsetLabel.Location = new Point(3, 180);
        this.miniSSCOffsetLabel.Name = "miniSSCOffsetLabel";
        this.miniSSCOffsetLabel.Size = new Size(82, 13);
        this.miniSSCOffsetLabel.TabIndex = 9;
        this.miniSSCOffsetLabel.Text = "Mini SSC offset:";
        this.miniSscOffset.Location = new Point(92, 178);
        this.miniSscOffset.Maximum = new Decimal(new int[4]
        {
        254,
        0,
        0,
        0
        });
        this.miniSscOffset.Name = "miniSscOffset";
        this.miniSscOffset.Size = new Size(48, 20);
        this.miniSscOffset.TabIndex = 10;
        this.toolTip1.SetToolTip((Control)this.miniSscOffset, "The Mini-SSC servo number for channel 0.");
        this.miniSscOffset.ValueChanged += new EventHandler(this.EnableApplyButton);
        this.miniSscOffset.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.neverSuspendBox.AutoSize = true;
        this.neverSuspendBox.Location = new Point(6, 233);
        this.neverSuspendBox.Name = "neverSuspendBox";
        this.neverSuspendBox.Size = new Size(189, 17);
        this.neverSuspendBox.TabIndex = 13;
        this.neverSuspendBox.Text = "Never sleep (ignore USB suspend)";
        this.neverSuspendBox.UseVisualStyleBackColor = true;
        this.neverSuspendBox.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.serialModeUsbDualPort.AutoSize = true;
        this.serialModeUsbDualPort.Checked = true;
        this.serialModeUsbDualPort.Location = new Point(6, 25);
        this.serialModeUsbDualPort.Name = "serialModeUsbDualPort";
        this.serialModeUsbDualPort.Size = new Size(94, 17);
        this.serialModeUsbDualPort.TabIndex = 1;
        this.serialModeUsbDualPort.TabStop = true;
        this.serialModeUsbDualPort.Text = "USB Dual Port";
        this.toolTip1.SetToolTip((Control)this.serialModeUsbDualPort, "In this mode, commands can be sent to the Maestro on\r\na virtual COM port, and a second port is available for\r\ncommunication with other devices on the TTL serial port.");
        this.serialModeUsbDualPort.UseVisualStyleBackColor = true;
        this.serialModeUsbDualPort.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.serialModeUsbChained.AutoSize = true;
        this.serialModeUsbChained.Location = new Point(6, 48);
        this.serialModeUsbChained.Name = "serialModeUsbChained";
        this.serialModeUsbChained.Size = new Size(89, 17);
        this.serialModeUsbChained.TabIndex = 2;
        this.serialModeUsbChained.Tag = (object)"a";
        this.serialModeUsbChained.Text = "USB Chained";
        this.toolTip1.SetToolTip((Control)this.serialModeUsbChained, "In this mode, commands can be sent to the Maestro on\r\na virtual COM port, and the commands will be echoed\r\non the TTL serial port.");
        this.serialModeUsbChained.UseVisualStyleBackColor = true;
        this.serialModeUsbChained.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.label25.AutoSize = true;
        this.label25.Location = new Point(3, 154);
        this.label25.Name = "label25";
        this.label25.Size = new Size(84, 13);
        this.label25.TabIndex = 7;
        this.label25.Text = "Device Number:";
        this.serialDeviceNumber.Location = new Point(92, 152);
        this.serialDeviceNumber.Maximum = new Decimal(new int[4]
        {
        (int) sbyte.MaxValue,
        0,
        0,
        0
        });
        this.serialDeviceNumber.Name = "serialDeviceNumber";
        this.serialDeviceNumber.Size = new Size(48, 20);
        this.serialDeviceNumber.TabIndex = 8;
        this.toolTip1.SetToolTip((Control)this.serialDeviceNumber, "The device number for use with the Pololu protocol.");
        this.serialDeviceNumber.Value = new Decimal(new int[4]
        {
        11,
        0,
        0,
        0
        });
        this.serialDeviceNumber.ValueChanged += new EventHandler(this.EnableApplyButton);
        this.serialDeviceNumber.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.serialFixedBaud.Location = new Point(146, 71);
        this.serialFixedBaud.Maximum = new Decimal(new int[4]
        {
        250000,
        0,
        0,
        0
        });
        this.serialFixedBaud.Minimum = new Decimal(new int[4]
        {
        300,
        0,
        0,
        0
        });
        this.serialFixedBaud.Name = "serialFixedBaud";
        this.serialFixedBaud.Size = new Size(69, 20);
        this.serialFixedBaud.TabIndex = 5;
        this.serialFixedBaud.Value = new Decimal(new int[4]
        {
        115200,
        0,
        0,
        0
        });
        this.serialFixedBaud.ValueChanged += new EventHandler(this.EnableApplyButton);
        this.serialFixedBaud.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.serialModeUartDetect.AutoSize = true;
        this.serialModeUartDetect.Location = new Point(6, 94);
        this.serialModeUartDetect.Name = "serialModeUartDetect";
        this.serialModeUartDetect.Size = new Size(139, 17);
        this.serialModeUartDetect.TabIndex = 4;
        this.serialModeUartDetect.Tag = (object)"a";
        this.serialModeUartDetect.Text = "UART, detect baud rate";
        this.toolTip1.SetToolTip((Control)this.serialModeUartDetect, "In this mode, commands can be sent to the Maestro on\r\nthe TTL serial port.  Incoming byte are echoed to the virtual\r\nCOM port.\r\n");
        this.serialModeUartDetect.UseVisualStyleBackColor = true;
        this.serialModeUartDetect.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.enableCrcBox.AutoSize = true;
        this.enableCrcBox.Location = new Point(6, 128);
        this.enableCrcBox.Name = "enableCrcBox";
        this.enableCrcBox.Size = new Size(84, 17);
        this.enableCrcBox.TabIndex = 6;
        this.enableCrcBox.Text = "Enable CRC";
        this.enableCrcBox.UseVisualStyleBackColor = true;
        this.enableCrcBox.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.serialTimeoutLabel.AutoSize = true;
        this.serialTimeoutLabel.Location = new Point(3, 205);
        this.serialTimeoutLabel.Name = "serialTimeoutLabel";
        this.serialTimeoutLabel.Size = new Size(62, 13);
        this.serialTimeoutLabel.TabIndex = 11;
        this.serialTimeoutLabel.Text = "Timeout (s):";
        this.serialModeUartFixed.AutoSize = true;
        this.serialModeUartFixed.Location = new Point(6, 71);
        this.serialModeUartFixed.Name = "serialModeUartFixed";
        this.serialModeUartFixed.Size = new Size(134, 17);
        this.serialModeUartFixed.TabIndex = 3;
        this.serialModeUartFixed.Tag = (object)"a";
        this.serialModeUartFixed.Text = "UART, fixed baud rate:";
        this.toolTip1.SetToolTip((Control)this.serialModeUartFixed, "In this mode, commands can be sent to the Maestro on\r\nthe TTL serial port.  Incoming byte are echoed to the virtual\r\nCOM port.");
        this.serialModeUartFixed.UseVisualStyleBackColor = true;
        this.serialModeUartFixed.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.sequenceTab.Controls.Add((Control)this.label30);
        this.sequenceTab.Controls.Add((Control)this.loopBox);
        this.sequenceTab.Controls.Add((Control)this.stopSequenceButton);
        this.sequenceTab.Controls.Add((Control)this.modifyFrameButton);
        this.sequenceTab.Controls.Add((Control)this.frameDownButton);
        this.sequenceTab.Controls.Add((Control)this.frameUpButton);
        this.sequenceTab.Controls.Add((Control)this.deleteFrameButton);
        this.sequenceTab.Controls.Add((Control)this.editFrameButton);
        this.sequenceTab.Controls.Add((Control)this.label24);
        this.sequenceTab.Controls.Add((Control)this.renameSequenceButton);
        this.sequenceTab.Controls.Add((Control)this.label22);
        this.sequenceTab.Controls.Add((Control)this.sequenceList);
        this.sequenceTab.Controls.Add((Control)this.newSequenceButton);
        this.sequenceTab.Controls.Add((Control)this.deleteSequenceButton);
        this.sequenceTab.Controls.Add((Control)this.loadFrameButton);
        this.sequenceTab.Controls.Add((Control)this.playSequenceButton);
        this.sequenceTab.Controls.Add((Control)this.copyToScriptButton);
        this.sequenceTab.Controls.Add((Control)this.copyToSubroutinesButton);
        this.sequenceTab.Controls.Add((Control)this.frameListView);
        this.sequenceTab.Location = new Point(4, 22);
        this.sequenceTab.Name = "sequenceTab";
        this.sequenceTab.Padding = new Padding(3);
        this.sequenceTab.Size = new Size(815, 446);
        this.sequenceTab.TabIndex = 3;
        this.sequenceTab.Tag = (object)"4";
        this.sequenceTab.Text = "Sequence";
        this.sequenceTab.UseVisualStyleBackColor = true;
        this.label30.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.label30.Location = new Point(11, 430);
        this.label30.Name = "label30";
        this.label30.Size = new Size(271, 13);
        this.label30.TabIndex = 8;
        this.label30.Text = "Tip: select multiple frames with Ctrl+Click or Shift+Click.";
        this.label30.TextAlign = ContentAlignment.TopRight;
        this.loopBox.AutoSize = true;
        this.loopBox.Location = new Point(288, 70);
        this.loopBox.Name = "loopBox";
        this.loopBox.Size = new Size(89, 17);
        this.loopBox.TabIndex = 9;
        this.loopBox.Text = "Play in a loop";
        this.loopBox.UseVisualStyleBackColor = true;
        this.stopSequenceButton.Location = new Point(288, 122);
        this.stopSequenceButton.Name = "stopSequenceButton";
        this.stopSequenceButton.Size = new Size(119, 23);
        this.stopSequenceButton.TabIndex = 12;
        this.stopSequenceButton.Text = "Stop Se&quence";
        this.stopSequenceButton.UseVisualStyleBackColor = true;
        this.stopSequenceButton.Click += new EventHandler(this.stopSequenceButton_Click);
        this.modifyFrameButton.Location = new Point(413, 183);
        this.modifyFrameButton.Name = "modifyFrameButton";
        this.modifyFrameButton.Size = new Size(146, 23);
        this.modifyFrameButton.TabIndex = 14;
        this.modifyFrameButton.Text = "Sa&ve Over Current Frame";
        this.toolTip1.SetToolTip((Control)this.modifyFrameButton, "Copies the current servo positions\r\nfrom the status tab in to this frame.");
        this.modifyFrameButton.UseVisualStyleBackColor = true;
        this.modifyFrameButton.Click += new EventHandler(this.modifyFrameButton_Click);
        this.frameDownButton.Location = new Point(288, 294);
        this.frameDownButton.Name = "frameDownButton";
        this.frameDownButton.Size = new Size(140, 23);
        this.frameDownButton.TabIndex = 18;
        this.frameDownButton.Text = "Move Frame Dow&n";
        this.frameDownButton.UseVisualStyleBackColor = true;
        this.frameDownButton.Click += new EventHandler(this.frameDownButton_Click);
        this.frameUpButton.Location = new Point(288, 265);
        this.frameUpButton.Name = "frameUpButton";
        this.frameUpButton.Size = new Size(140, 23);
        this.frameUpButton.TabIndex = 17;
        this.frameUpButton.Text = "Move Frame &Up";
        this.frameUpButton.UseVisualStyleBackColor = true;
        this.frameUpButton.Click += new EventHandler(this.frameUpButton_Click);
        this.deleteFrameButton.Location = new Point(413, 212);
        this.deleteFrameButton.Name = "deleteFrameButton";
        this.deleteFrameButton.Size = new Size(146, 23);
        this.deleteFrameButton.TabIndex = 16;
        this.deleteFrameButton.Text = "Delete Fr&ame";
        this.deleteFrameButton.UseVisualStyleBackColor = true;
        this.deleteFrameButton.Click += new EventHandler(this.deleteFrameButton_Click);
        this.editFrameButton.Location = new Point(288, 212);
        this.editFrameButton.Name = "editFrameButton";
        this.editFrameButton.Size = new Size(119, 23);
        this.editFrameButton.TabIndex = 15;
        this.editFrameButton.Text = "Fra&me properties...";
        this.editFrameButton.UseVisualStyleBackColor = true;
        this.editFrameButton.Click += new EventHandler(this.editFrameButton_Click);
        this.label24.AutoSize = true;
        this.label24.Location = new Point(8, 49);
        this.label24.Name = "label24";
        this.label24.Size = new Size(44, 13);
        this.label24.TabIndex = 6;
        this.label24.Text = "Frames:";
        this.renameSequenceButton.Location = new Point(279, 20);
        this.renameSequenceButton.Name = "renameSequenceButton";
        this.renameSequenceButton.Size = new Size(61, 23);
        this.renameSequenceButton.TabIndex = 2;
        this.renameSequenceButton.Text = "&Rename";
        this.renameSequenceButton.UseVisualStyleBackColor = true;
        this.renameSequenceButton.Click += new EventHandler(this.renameSequenceButton_Click);
        this.label22.AutoSize = true;
        this.label22.Location = new Point(8, 25);
        this.label22.Name = "label22";
        this.label22.Size = new Size(59, 13);
        this.label22.TabIndex = 0;
        this.label22.Text = "Sequence:";
        this.sequenceList.DropDownStyle = ComboBoxStyle.DropDownList;
        this.sequenceList.FormattingEnabled = true;
        this.sequenceList.Location = new Point(73, 22);
        this.sequenceList.Name = "sequenceList";
        this.sequenceList.Size = new Size(200, 21);
        this.sequenceList.TabIndex = 1;
        this.sequenceList.SelectedIndexChanged += new EventHandler(this.sequenceList_SelectedIndexChanged);
        this.scriptTab.Controls.Add((Control)this.stackListView);
        this.scriptTab.Controls.Add((Control)this.scriptBytesLabel);
        this.scriptTab.Controls.Add((Control)this.callStackLabel);
        this.scriptTab.Controls.Add((Control)this.restartScriptButton);
        this.scriptTab.Controls.Add((Control)this.listingButton);
        this.scriptTab.Controls.Add((Control)this.runScriptOnStartupCheckbox);
        this.scriptTab.Controls.Add((Control)this.label21);
        this.scriptTab.Controls.Add((Control)this.label20);
        this.scriptTab.Controls.Add((Control)this.scrollToFollowCheckBox);
        this.scriptTab.Controls.Add((Control)this.stepScriptButton);
        this.scriptTab.Controls.Add((Control)this.runScriptButton);
        this.scriptTab.Controls.Add((Control)this.stopScriptButton);
        this.scriptTab.Controls.Add((Control)this.scriptLineNumberBox);
        this.scriptTab.Controls.Add((Control)this.scriptTextBox);
        this.scriptTab.Location = new Point(4, 22);
        this.scriptTab.Name = "scriptTab";
        this.scriptTab.Padding = new Padding(3);
        this.scriptTab.Size = new Size(815, 446);
        this.scriptTab.TabIndex = 2;
        this.scriptTab.Tag = (object)"5";
        this.scriptTab.Text = "Script";
        this.scriptTab.UseVisualStyleBackColor = true;
        this.scriptBytesLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.scriptBytesLabel.AutoSize = true;
        this.scriptBytesLabel.ForeColor = SystemColors.ControlText;
        this.scriptBytesLabel.Location = new Point(277, 5);
        this.scriptBytesLabel.Name = "scriptBytesLabel";
        this.scriptBytesLabel.Size = new Size(160, 13);
        this.scriptBytesLabel.TabIndex = 2;
        this.scriptBytesLabel.Text = "Script: 1000  of 1024 bytes used";
        this.callStackLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.callStackLabel.AutoSize = true;
        this.callStackLabel.ForeColor = SystemColors.ControlText;
        this.callStackLabel.Location = new Point(477, 5);
        this.callStackLabel.Name = "callStackLabel";
        this.callStackLabel.Size = new Size(148, 13);
        this.callStackLabel.TabIndex = 3;
        this.callStackLabel.Text = "Subroutines: 1/10 levels used";
        this.restartScriptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.restartScriptButton.BackColor = Color.Transparent;
        this.restartScriptButton.ForeColor = SystemColors.ControlText;
        this.restartScriptButton.Location = new Point(249, 417);
        this.restartScriptButton.Name = "restartScriptButton";
        this.restartScriptButton.Size = new Size(92, 23);
        this.restartScriptButton.TabIndex = 8;
        this.restartScriptButton.Text = "Restart Script";
        this.restartScriptButton.UseVisualStyleBackColor = false;
        this.restartScriptButton.Click += new EventHandler(this.restartScriptButton_Click);
        this.listingButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.listingButton.BackColor = Color.Transparent;
        this.listingButton.ForeColor = SystemColors.ControlText;
        this.listingButton.Location = new Point(535, 417);
        this.listingButton.Name = "listingButton";
        this.listingButton.Size = new Size(142, 23);
        this.listingButton.TabIndex = 10;
        this.listingButton.Text = "&View Compiled Code...";
        this.listingButton.UseVisualStyleBackColor = false;
        this.listingButton.Click += new EventHandler(this.listingButton_Click);
        this.runScriptOnStartupCheckbox.AutoSize = true;
        this.runScriptOnStartupCheckbox.BackColor = Color.Transparent;
        this.runScriptOnStartupCheckbox.Location = new Point(40, 4);
        this.runScriptOnStartupCheckbox.Name = "runScriptOnStartupCheckbox";
        this.runScriptOnStartupCheckbox.Size = new Size(124, 17);
        this.runScriptOnStartupCheckbox.TabIndex = 1;
        this.runScriptOnStartupCheckbox.Text = "Run script on startup";
        this.runScriptOnStartupCheckbox.UseVisualStyleBackColor = true;
        this.runScriptOnStartupCheckbox.CheckedChanged += new EventHandler(this.EnableApplyButton);
        this.label21.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.label21.AutoSize = true;
        this.label21.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.label21.Location = new Point(680, 5);
        this.label21.Name = "label21";
        this.label21.Size = new Size(35, 13);
        this.label21.TabIndex = 11;
        this.label21.Text = "Stack";
        this.label20.AutoSize = true;
        this.label20.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.label20.Location = new Point(2, 5);
        this.label20.Name = "label20";
        this.label20.Size = new Size(32, 13);
        this.label20.TabIndex = 0;
        this.label20.Text = "Code";
        this.scrollToFollowCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.scrollToFollowCheckBox.AutoSize = true;
        this.scrollToFollowCheckBox.Location = new Point(347, 421);
        this.scrollToFollowCheckBox.Name = "scrollToFollowCheckBox";
        this.scrollToFollowCheckBox.Size = new Size(122, 17);
        this.scrollToFollowCheckBox.TabIndex = 9;
        this.scrollToFollowCheckBox.Text = "Scroll to follow script";
        this.scrollToFollowCheckBox.UseVisualStyleBackColor = true;
        this.stepScriptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.stepScriptButton.BackColor = Color.Transparent;
        this.stepScriptButton.Enabled = false;
        this.stepScriptButton.ForeColor = SystemColors.ControlText;
        this.stepScriptButton.Location = new Point(168, 417);
        this.stepScriptButton.Name = "stepScriptButton";
        this.stepScriptButton.Size = new Size(75, 23);
        this.stepScriptButton.TabIndex = 7;
        this.stepScriptButton.Text = "Ste&p Script";
        this.stepScriptButton.UseVisualStyleBackColor = false;
        this.stepScriptButton.Click += new EventHandler(this.stepScriptButton_Click);
        this.runScriptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.runScriptButton.BackColor = Color.Transparent;
        this.runScriptButton.Enabled = false;
        this.runScriptButton.ForeColor = SystemColors.ControlText;
        this.runScriptButton.Location = new Point(87, 417);
        this.runScriptButton.Name = "runScriptButton";
        this.runScriptButton.Size = new Size(75, 23);
        this.runScriptButton.TabIndex = 6;
        this.runScriptButton.Text = "&Run Script";
        this.runScriptButton.UseVisualStyleBackColor = false;
        this.runScriptButton.Click += new EventHandler(this.runScriptButton_Click);
        this.stopScriptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.stopScriptButton.BackColor = Color.Red;
        this.stopScriptButton.ForeColor = Color.White;
        this.stopScriptButton.Location = new Point(6, 417);
        this.stopScriptButton.Name = "stopScriptButton";
        this.stopScriptButton.Size = new Size(75, 23);
        this.stopScriptButton.TabIndex = 5;
        this.stopScriptButton.Text = "&Stop Script";
        this.stopScriptButton.UseVisualStyleBackColor = false;
        this.stopScriptButton.Click += new EventHandler(this.stopScriptButton_Click);
        this.applyProgressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.applyProgressBar.Location = new Point(557, 492);
        this.applyProgressBar.Name = "applyProgressBar";
        this.applyProgressBar.Size = new Size(100, 23);
        this.applyProgressBar.Step = 1;
        this.applyProgressBar.TabIndex = 7;
        this.applyProgressBar.Visible = false;
        this.firmwareVersionLabel.AutoSize = true;
        this.firmwareVersionLabel.ForeColor = SystemColors.ControlText;
        this.firmwareVersionLabel.Location = new Point(199, 34);
        this.firmwareVersionLabel.Name = "firmwareVersionLabel";
        this.firmwareVersionLabel.Size = new Size(107, 13);
        this.firmwareVersionLabel.TabIndex = 2;
        this.firmwareVersionLabel.Text = "Firmware version: 1.0";
        this.firmwareVersionLabel.Visible = false;
        this.connectionLostLabel.AutoSize = true;
        this.connectionLostLabel.ForeColor = Color.Red;
        this.connectionLostLabel.Location = new Point(199, 34);
        this.connectionLostLabel.Name = "connectionLostLabel";
        this.connectionLostLabel.Size = new Size(216, 13);
        this.connectionLostLabel.TabIndex = 30;
        this.connectionLostLabel.Text = "Connection to Maestro #00000211 was lost.";
        this.connectionLostLabel.Visible = false;
        this.label23.AutoSize = true;
        this.label23.Location = new Point(9, 34);
        this.label23.Name = "label23";
        this.label23.Size = new Size(74, 13);
        this.label23.TabIndex = 0;
        this.label23.Text = "Connected to:";
        this.deviceList.DisplayMember = "text";
        this.deviceList.DropDownStyle = ComboBoxStyle.DropDownList;
        this.deviceList.FormattingEnabled = true;
        this.deviceList.Location = new Point(89, 31);
        this.deviceList.Name = "deviceList";
        this.deviceList.Size = new Size(104, 21);
        this.deviceList.TabIndex = 1;
        this.deviceList.SelectedIndexChanged += new EventHandler(this.deviceList_SelectedIndexChanged);
        this.menuStrip1.Items.AddRange(new ToolStripItem[4]
        {
        (ToolStripItem) this.fileToolStripMenuItem,
        (ToolStripItem) this.devicToolStripMenuItem,
        (ToolStripItem) this.editToolStripMenuItem,
        (ToolStripItem) this.helpToolStripMenuItem
        });
        this.menuStrip1.Location = new Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Size = new Size(823, 24);
        this.menuStrip1.TabIndex = 0;
        this.menuStrip1.Text = "menuStrip1";
        this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[3]
        {
        (ToolStripItem) this.saveSettingsToolStripMenuItem,
        (ToolStripItem) this.loadSettingsToolStripMenuItem,
        (ToolStripItem) this.exitToolStripMenuItem
        });
        this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        this.fileToolStripMenuItem.Size = new Size(37, 20);
        this.fileToolStripMenuItem.Text = "&File";
        this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
        this.saveSettingsToolStripMenuItem.ShortcutKeys = Keys.S | Keys.Control;
        this.saveSettingsToolStripMenuItem.Size = new Size(218, 22);
        this.saveSettingsToolStripMenuItem.Text = "&Save settings file...";
        this.saveSettingsToolStripMenuItem.Click += new EventHandler(this.saveSettingsToolStripMenuItem_Click);
        this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
        this.loadSettingsToolStripMenuItem.ShortcutKeys = Keys.O | Keys.Control;
        this.loadSettingsToolStripMenuItem.Size = new Size(218, 22);
        this.loadSettingsToolStripMenuItem.Text = "&Open settings file...";
        this.loadSettingsToolStripMenuItem.Click += new EventHandler(this.loadSettingsToolStripMenuItem_Click);
        this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        this.exitToolStripMenuItem.Size = new Size(218, 22);
        this.exitToolStripMenuItem.Text = "E&xit";
        this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
        this.devicToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[4]
        {
        (ToolStripItem) this.restartDeviceToolStripMenuItem,
        (ToolStripItem) this.reloadSettingsToolStripMenuItem,
        (ToolStripItem) this.resetToFactorySettingsToolStripMenuItem,
        (ToolStripItem) this.upgradeFirmwareToolStripMenuItem
        });
        this.devicToolStripMenuItem.Name = "devicToolStripMenuItem";
        this.devicToolStripMenuItem.Size = new Size(54, 20);
        this.devicToolStripMenuItem.Text = "&Device";
        this.restartDeviceToolStripMenuItem.Name = "restartDeviceToolStripMenuItem";
        this.restartDeviceToolStripMenuItem.Size = new Size(209, 22);
        this.restartDeviceToolStripMenuItem.Text = "Res&tart Device";
        this.restartDeviceToolStripMenuItem.Click += new EventHandler(this.restartDeviceToolStripMenuItem_Click);
        this.reloadSettingsToolStripMenuItem.Name = "reloadSettingsToolStripMenuItem";
        this.reloadSettingsToolStripMenuItem.Size = new Size(209, 22);
        this.reloadSettingsToolStripMenuItem.Text = "Re&load Settings";
        this.reloadSettingsToolStripMenuItem.Click += new EventHandler(this.ReloadButton_Click);
        this.resetToFactorySettingsToolStripMenuItem.Name = "resetToFactorySettingsToolStripMenuItem";
        this.resetToFactorySettingsToolStripMenuItem.Size = new Size(209, 22);
        this.resetToFactorySettingsToolStripMenuItem.Text = "&Reset to default settings...";
        this.resetToFactorySettingsToolStripMenuItem.Click += new EventHandler(this.resetToFactorySettingsToolStripMenuItem_Click);
        this.upgradeFirmwareToolStripMenuItem.Name = "upgradeFirmwareToolStripMenuItem";
        this.upgradeFirmwareToolStripMenuItem.Size = new Size(209, 22);
        this.upgradeFirmwareToolStripMenuItem.Text = "&Upgrade firmware...";
        this.upgradeFirmwareToolStripMenuItem.Click += new EventHandler(this.upgradeFirmwareToolStripMenuItem_Click);
        this.editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[3]
        {
        (ToolStripItem) this.cutToolStripMenuItem,
        (ToolStripItem) this.copyToolStripMenuItem,
        (ToolStripItem) this.pasteToolStripMenuItem
        });
        this.editToolStripMenuItem.Name = "editToolStripMenuItem";
        this.editToolStripMenuItem.Size = new Size(39, 20);
        this.editToolStripMenuItem.Text = "&Edit";
        this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
        this.cutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+X";
        this.cutToolStripMenuItem.Size = new Size(144, 22);
        this.cutToolStripMenuItem.Text = "Cu&t";
        this.cutToolStripMenuItem.Click += new EventHandler(this.cutToolStripMenuItem_Click);
        this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        this.copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
        this.copyToolStripMenuItem.Size = new Size(144, 22);
        this.copyToolStripMenuItem.Text = "&Copy";
        this.copyToolStripMenuItem.Click += new EventHandler(this.copyToolStripMenuItem_Click);
        this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
        this.pasteToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
        this.pasteToolStripMenuItem.Size = new Size(144, 22);
        this.pasteToolStripMenuItem.Text = "&Paste";
        this.pasteToolStripMenuItem.Click += new EventHandler(this.pasteToolStripMenuItem_Click);
        this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
        {
        (ToolStripItem) this.aboutToolStripMenuItem,
        (ToolStripItem) this.goToOnlineDocumentationToolStripMenuItem
        });
        this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        this.helpToolStripMenuItem.Size = new Size(44, 20);
        this.helpToolStripMenuItem.Text = "&Help";
        this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        this.aboutToolStripMenuItem.Size = new Size(203, 22);
        this.aboutToolStripMenuItem.Text = "&About...";
        this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
        this.goToOnlineDocumentationToolStripMenuItem.Name = "goToOnlineDocumentationToolStripMenuItem";
        this.goToOnlineDocumentationToolStripMenuItem.Size = new Size(203, 22);
        this.goToOnlineDocumentationToolStripMenuItem.Text = "&Online documentation...";
        this.goToOnlineDocumentationToolStripMenuItem.Click += new EventHandler(this.goToOnlineDocumentationToolStripMenuItem_Click);
        this.openSettingsFileDialog.DefaultExt = "txt";
        this.openSettingsFileDialog.FileName = "maestro_settings.txt";
        this.openSettingsFileDialog.Filter = "Text files|*.txt|All files|*.*";
        this.openSettingsFileDialog.Title = "Open settings file";
        this.saveSettingsFileDialog.DefaultExt = "txt";
        this.saveSettingsFileDialog.FileName = "maestro_settings.txt";
        this.saveSettingsFileDialog.Filter = "Text files|*.txt|All files|*.*";
        this.saveSettingsFileDialog.Title = "Save settings file";
        this.label9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.label9.AutoSize = true;
        this.label9.ForeColor = SystemColors.ControlText;
        this.label9.Location = new Point(636, 34);
        this.label9.Name = "label9";
        this.label9.Size = new Size(59, 13);
        this.label9.TabIndex = 3;
        this.label9.Text = "Error code:";
        this.errorCode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.errorCode.BackColor = SystemColors.Control;
        this.errorCode.BorderStyle = BorderStyle.None;
        this.errorCode.ForeColor = Color.Black;
        this.errorCode.Location = new Point(701, 34);
        this.errorCode.Name = "errorCode";
        this.errorCode.ReadOnly = true;
        this.errorCode.Size = new Size(40, 13);
        this.errorCode.TabIndex = 4;
        this.errorCode.Text = "N/A";
        this.toolTip1.SetToolTip((Control)this.errorCode, "Displays the currently active error bits.  See the Errors\r\ntab for details.");
        this.pwmPeriod.Increment = new Decimal(new int[4]
        {
        4,
        0,
        0,
        0
        });
        this.pwmPeriod.Location = new Point(67, 75);
        this.pwmPeriod.MantissaBits = 0;
        this.pwmPeriod.Maximum = new Decimal(new int[4]
        {
        16384,
        0,
        0,
        0
        });
        this.pwmPeriod.Minimum = new Decimal(new int[4]
        {
        4,
        0,
        0,
        0
        });
        this.pwmPeriod.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.pwmPeriod.Name = "pwmPeriod";
        this.pwmPeriod.Size = new Size(64, 20);
        this.pwmPeriod.TabIndex = 2;
        this.pwmPeriod.TextAlign = HorizontalAlignment.Right;
        this.toolTip1.SetToolTip((Control)this.pwmPeriod, "Sets the time between the start of one pulse and the\r\nstart of the next pulse on the PWM line.");
        this.pwmPeriod.Value = new Decimal(new int[4]
        {
        20,
        0,
        0,
        0
        });
        this.pwmPeriod.ValueChanged += new EventHandler(this.updatePWM);
        this.pwmDutyCycle.Location = new Point(67, 49);
        this.pwmDutyCycle.MantissaBits = 0;
        this.pwmDutyCycle.Maximum = new Decimal(new int[4]
        {
        16384,
        0,
        0,
        0
        });
        this.pwmDutyCycle.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.pwmDutyCycle.Name = "pwmDutyCycle";
        this.pwmDutyCycle.Size = new Size(64, 20);
        this.pwmDutyCycle.TabIndex = 1;
        this.pwmDutyCycle.TextAlign = HorizontalAlignment.Right;
        this.toolTip1.SetToolTip((Control)this.pwmDutyCycle, "Sets the duration of the high pulses on the PWM line.");
        this.pwmDutyCycle.Value = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.pwmDutyCycle.ValueChanged += new EventHandler(this.updatePWM);
        this.servoStatusControl0.acceleration = (ushort)0;
        this.servoStatusControl0.BackColor = Color.Transparent;
        this.servoStatusControl0.Enabled = false;
        this.servoStatusControl0.Location = new Point(0, 2);
        this.servoStatusControl0.Margin = new Padding(0, 2, 0, 2);
        this.servoStatusControl0.max = (ushort)8000;
        this.servoStatusControl0.min = (ushort)3968;
        this.servoStatusControl0.Name = "servoStatusControl0";
        this.servoStatusControl0.position = (ushort)6000;
        this.servoStatusControl0.servoName = "1";
        this.servoStatusControl0.servoNumber = 0;
        this.servoStatusControl0.Size = new Size(721, 24);
        this.servoStatusControl0.speed = (ushort)0;
        this.servoStatusControl0.TabIndex = 8;
        this.servoStatusControl0.target = (ushort)6000;
        this.servoStatusControl0.Visible = false;
        this.servoMultiplier.Location = new Point(122, 61);
        this.servoMultiplier.MantissaBits = 0;
        this.servoMultiplier.Maximum = new Decimal(new int[4]
        {
        256,
        0,
        0,
        0
        });
        this.servoMultiplier.Minimum = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.servoMultiplier.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.servoMultiplier.Name = "servoMultiplier";
        this.servoMultiplier.Size = new Size(44, 20);
        this.servoMultiplier.TabIndex = 5;
        this.servoMultiplier.TextAlign = HorizontalAlignment.Right;
        this.toolTip1.SetToolTip((Control)this.servoMultiplier, "Allows some channels to have a period increased by the multiplier.\r\nFor example, to get 250 Hz and 20 Hz, use a period of 4 and multiplier of 5.");
        this.servoMultiplier.Value = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.servoMultiplier.ValueChanged += new EventHandler(this.periodMultiplier_ValueChanged);
        this.servoMultiplier.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.miniMaestroServoPeriod.Location = new Point(122, 30);
        this.miniMaestroServoPeriod.MantissaBits = 0;
        this.miniMaestroServoPeriod.Maximum = new Decimal(new int[4]
        {
        1000,
        0,
        0,
        0
        });
        this.miniMaestroServoPeriod.Minimum = new Decimal(new int[4]
        {
        3,
        0,
        0,
        0
        });
        this.miniMaestroServoPeriod.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.miniMaestroServoPeriod.Name = "miniMaestroServoPeriod";
        this.miniMaestroServoPeriod.Size = new Size(44, 20);
        this.miniMaestroServoPeriod.TabIndex = 3;
        this.miniMaestroServoPeriod.TextAlign = HorizontalAlignment.Right;
        this.toolTip1.SetToolTip((Control)this.miniMaestroServoPeriod, "Sets the period for servo pulses on all of the channels.\r\nIf you don't know that you need this, leave it at the default\r\nof 20 ms.");
        this.miniMaestroServoPeriod.Value = new Decimal(new int[4]
        {
        20,
        0,
        0,
        0
        });
        this.miniMaestroServoPeriod.ValueChanged += new EventHandler(this.miniMaestroServoPeriod_ValueChanged);
        this.miniMaestroServoPeriod.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.servoControl0.acceleration = (byte)0;
        this.servoControl0.Enabled = false;
        this.servoControl0.home = (ushort)3968;
        this.servoControl0.homeMode = HomeMode.Off;
        this.servoControl0.Location = new Point(0, 2);
        this.servoControl0.Margin = new Padding(0, 2, 0, 2);
        this.servoControl0.max = (ushort)0;
        this.servoControl0.maxLimit = (ushort)0;
        this.servoControl0.min = (ushort)3968;
        this.servoControl0.mode = ChannelMode.Servo;
        this.servoControl0.Name = "servoControl0";
        this.servoControl0.neutral = (ushort)3968;
        this.servoControl0.onlyAllowIO = false;
        this.servoControl0.range = (ushort)1905;
        this.servoControl0.servoName = "";
        this.servoControl0.servoNumber = 0;
        this.servoControl0.Size = new Size(789, 23);
        this.servoControl0.speed = (ushort)0;
        this.servoControl0.TabIndex = 10;
        this.servoControl0.Visible = false;
        this.periodUpDown.Location = new Point(122, 56);
        this.periodUpDown.MantissaBits = 0;
        this.periodUpDown.Maximum = new Decimal(new int[4]
        {
        30,
        0,
        0,
        0
        });
        this.periodUpDown.Minimum = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.periodUpDown.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.periodUpDown.Name = "periodUpDown";
        this.periodUpDown.Size = new Size(44, 20);
        this.periodUpDown.TabIndex = 3;
        this.periodUpDown.TextAlign = HorizontalAlignment.Right;
        this.toolTip1.SetToolTip((Control)this.periodUpDown, "Sets the period for servo pulses on all of the channels.\r\nIf you don't know that you need this, leave it at the default\r\nof 20 ms.");
        this.periodUpDown.Value = new Decimal(new int[4]
        {
        20,
        0,
        0,
        0
        });
        this.periodUpDown.ValueChanged += new EventHandler(this.periodUpDown_ValueChanged);
        this.periodUpDown.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.servosAvailableUpDown.Location = new Point(122, 30);
        this.servosAvailableUpDown.MantissaBits = 0;
        this.servosAvailableUpDown.Maximum = new Decimal(new int[4]
        {
        6,
        0,
        0,
        0
        });
        this.servosAvailableUpDown.Minimum = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.servosAvailableUpDown.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.servosAvailableUpDown.Name = "servosAvailableUpDown";
        this.servosAvailableUpDown.Size = new Size(44, 20);
        this.servosAvailableUpDown.TabIndex = 1;
        this.servosAvailableUpDown.TextAlign = HorizontalAlignment.Right;
        this.toolTip1.SetToolTip((Control)this.servosAvailableUpDown, componentResourceManager.GetString("servosAvailableUpDown.ToolTip"));
        this.servosAvailableUpDown.Value = new Decimal(new int[4]
        {
        6,
        0,
        0,
        0
        });
        this.servosAvailableUpDown.ValueChanged += new EventHandler(this.servosAvailableUpDown_ValueChanged);
        this.servosAvailableUpDown.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.serialTimeout.DecimalPlaces = 2;
        this.serialTimeout.Increment = new Decimal(new int[4]
        {
        1,
        0,
        0,
        131072
        });
        this.serialTimeout.Location = new Point(92, 203);
        this.serialTimeout.MantissaBits = 0;
        this.serialTimeout.Maximum = new Decimal(new int[4]
        {
        655350,
        0,
        0,
        196608
        });
        this.serialTimeout.MinimumIncrement = new Decimal(new int[4]
        {
        1,
        0,
        0,
        0
        });
        this.serialTimeout.Name = "serialTimeout";
        this.serialTimeout.Size = new Size(83, 20);
        this.serialTimeout.TabIndex = 12;
        this.toolTip1.SetToolTip((Control)this.serialTimeout, "Triggers an error if a serial command is not recieved within this time.");
        this.serialTimeout.ValueChanged += new EventHandler(this.EnableApplyButton);
        this.serialTimeout.KeyPress += new KeyPressEventHandler(this.EnableApplyButton);
        this.stackListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        this.stackListView.Columns.AddRange(new ColumnHeader[3]
        {
        this.stackDummyColumn,
        this.stackIndexColumn,
        this.valueIndexColumn
        });
        this.stackListView.FullRowSelect = true;
        this.stackListView.Location = new Point(683, 21);
        this.stackListView.Name = "stackListView";
        this.stackListView.Size = new Size(124, 417);
        this.stackListView.TabIndex = 12;
        this.stackListView.UseCompatibleStateImageBehavior = false;
        this.stackListView.View = View.Details;
        this.stackListView.KeyDown += new KeyEventHandler(this.stackListView_KeyDown);
        this.stackDummyColumn.Text = "";
        this.stackDummyColumn.Width = 1;
        this.stackIndexColumn.Text = "#";
        this.stackIndexColumn.TextAlign = HorizontalAlignment.Right;
        this.stackIndexColumn.Width = 34;
        this.valueIndexColumn.Text = "Value";
        this.valueIndexColumn.TextAlign = HorizontalAlignment.Right;
        this.valueIndexColumn.Width = 63;
        this.scriptLineNumberBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        this.scriptLineNumberBox.BackColor = SystemColors.Window;
        this.scriptLineNumberBox.Font = new Font("Lucida Console", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.scriptLineNumberBox.ForeColor = Color.Gray;
        this.scriptLineNumberBox.Location = new Point(5, 23);
        this.scriptLineNumberBox.Name = "scriptLineNumberBox";
        this.scriptLineNumberBox.Size = new Size(38, 370);
        this.scriptLineNumberBox.TabIndex = 4;
        this.scriptLineNumberBox.Text = "lineNumberBox1";
        this.scriptLineNumberBox.topY = 0;
        this.scriptTextBox.AcceptsTab = true;
        this.scriptTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.scriptTextBox.color0 = Color.Black;
        this.scriptTextBox.color1 = Color.Blue;
        this.scriptTextBox.color2 = Color.SaddleBrown;
        this.scriptTextBox.color3 = Color.Green;
        this.scriptTextBox.color4 = Color.Goldenrod;
        this.scriptTextBox.commentColor = Color.FromArgb(65, 129, 173);
        this.scriptTextBox.Font = new Font("Lucida Console", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.scriptTextBox.ForeColor = Color.DarkCyan;
        this.scriptTextBox.lineNumberBox = (LineNumberBox)null;
        this.scriptTextBox.Location = new Point(3, 21);
        this.scriptTextBox.Name = "scriptTextBox";
        this.scriptTextBox.pointerActive = true;
        this.scriptTextBox.pointerColor = Color.HotPink;
        this.scriptTextBox.pointerColumn = 5;
        this.scriptTextBox.pointerLine = 1;
        this.scriptTextBox.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
        this.scriptTextBox.Size = new Size(674, 390);
        this.scriptTextBox.TabIndex = 0;
        this.scriptTextBox.Text = "";
        this.scriptTextBox.WordWrap = false;
        this.scriptTextBox.TextChanged += new EventHandler(this.scriptTextBox_TextChanged);
        this.servoStatusControl1.acceleration = (ushort)0;
        this.servoStatusControl1.BackColor = Color.Transparent;
        this.servoStatusControl1.Location = new Point(0, 2);
        this.servoStatusControl1.Margin = new Padding(0, 2, 0, 2);
        this.servoStatusControl1.max = (ushort)8000;
        this.servoStatusControl1.min = (ushort)3968;
        this.servoStatusControl1.Name = "servoStatusControl1";
        this.servoStatusControl1.position = (ushort)6000;
        this.servoStatusControl1.servoName = "1";
        this.servoStatusControl1.servoNumber = 0;
        this.servoStatusControl1.Size = new Size(811, 24);
        this.servoStatusControl1.speed = (ushort)0;
        this.servoStatusControl1.TabIndex = 8;
        this.servoStatusControl1.target = (ushort)6000;
        this.AutoScaleDimensions = new SizeF(6f, 13f);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(823, 522);
        this.Controls.Add((Control)this.errorCode);
        this.Controls.Add((Control)this.label9);
        this.Controls.Add((Control)this.menuStrip1);
        this.Controls.Add((Control)this.firmwareVersionLabel);
        this.Controls.Add((Control)this.applyProgressBar);
        this.Controls.Add((Control)this.connectionLostLabel);
        this.Controls.Add((Control)this.label23);
        this.Controls.Add((Control)this.deviceList);
        this.Controls.Add((Control)this.tabs);
        this.Controls.Add((Control)this.ApplyButton);
        this.Controls.Add((Control)this.saveFrameButton);
        this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        this.Name = nameof(Form1);
        this.Text = "Pololu Maestro Control Center";
        this.Load += new EventHandler(this.Form1_Load);
        this.Shown += new EventHandler(this.Form1_Shown);
        this.FormClosed += new FormClosedEventHandler(this.Form1_FormClosed);
        this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        this.tabs.ResumeLayout(false);
        this.statusTab.ResumeLayout(false);
        this.statusTab.PerformLayout();
        this.pwmGroupBox.ResumeLayout(false);
        this.pwmGroupBox.PerformLayout();
        this.servoStatusFlowLayoutPanel.ResumeLayout(false);
        this.errorsTab.ResumeLayout(false);
        this.errorsTab.PerformLayout();
        this.settingsTab.ResumeLayout(false);
        this.settingsTab.PerformLayout();
        this.miniMaestroAdvancedBox.ResumeLayout(false);
        this.miniMaestroAdvancedBox.PerformLayout();
        this.servoControlFlowLayoutPanel.ResumeLayout(false);
        this.microMaestroAdvancedBox.ResumeLayout(false);
        this.microMaestroAdvancedBox.PerformLayout();
        this.serialSettingsTab.ResumeLayout(false);
        this.serialSettingsTab.PerformLayout();
        this.miniSscOffset.EndInit();
        this.serialDeviceNumber.EndInit();
        this.serialFixedBaud.EndInit();
        this.sequenceTab.ResumeLayout(false);
        this.sequenceTab.PerformLayout();
        this.scriptTab.ResumeLayout(false);
        this.scriptTab.PerformLayout();
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.pwmPeriod.EndInit();
        this.pwmDutyCycle.EndInit();
        this.servoMultiplier.EndInit();
        this.miniMaestroServoPeriod.EndInit();
        this.periodUpDown.EndInit();
        this.servosAvailableUpDown.EndInit();
        this.serialTimeout.EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();


    }

    #endregion
}

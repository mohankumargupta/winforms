// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Sequencer.FrameEditWindow
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace Pololu.Usc.Sequencer
{
    public class FrameEditWindow : Form
    {
        private IContainer components;
        private TextBox frameNameBox;
        private Button cancelButton;
        private Button okButton;
        private Label sequenceNameLabel;
        private Label label1;
        private NumericUpDown durationBox;

        private FrameEditWindow()
        {
            this.InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }

        public static DialogResult launch(IList<Frame> frames)
        {
            FrameEditWindow frameEditWindow = new FrameEditWindow();
            if (frames.Count == 1)
                frameEditWindow.frameNameBox.Text = frames[0].name;
            else
                frameEditWindow.frameNameBox.Enabled = false;
            frameEditWindow.durationBox.Value = (Decimal)frames[0].length_ms;
            int num = (int)frameEditWindow.ShowDialog();
            if (frameEditWindow.DialogResult == DialogResult.OK)
            {
                foreach (Frame frame in (IEnumerable<Frame>)frames)
                {
                    if (frames.Count == 1)
                        frame.name = frameEditWindow.frameNameBox.Text.Trim();
                    frame.length_ms = (ushort)frameEditWindow.durationBox.Value;
                }
            }
            return frameEditWindow.DialogResult;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FrameEditWindow_Shown(object sender, EventArgs e)
        {
            if (this.frameNameBox.Enabled)
                this.frameNameBox.Focus();
            else
                this.durationBox.Focus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FrameEditWindow));
            this.frameNameBox = new TextBox();
            this.cancelButton = new Button();
            this.okButton = new Button();
            this.sequenceNameLabel = new Label();
            this.label1 = new Label();
            this.durationBox = new NumericUpDown();
            this.durationBox.BeginInit();
            this.SuspendLayout();
            this.frameNameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.frameNameBox.Location = new Point(87, 6);
            this.frameNameBox.Name = "frameNameBox";
            this.frameNameBox.Size = new Size(265, 20);
            this.frameNameBox.TabIndex = 5;
            this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(277, 56);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
            this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.okButton.Location = new Point(196, 56);
            this.okButton.Name = "okButton";
            this.okButton.Size = new Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new EventHandler(this.okButton_Click);
            this.sequenceNameLabel.AutoSize = true;
            this.sequenceNameLabel.Location = new Point(9, 9);
            this.sequenceNameLabel.Name = "sequenceNameLabel";
            this.sequenceNameLabel.Size = new Size(38, 13);
            this.sequenceNameLabel.TabIndex = 6;
            this.sequenceNameLabel.Text = "Name:";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 34);
            this.label1.Name = "label1";
            this.label1.Size = new Size(72, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Duration (ms):";
            this.durationBox.Increment = new Decimal(new int[4]
            {
        100,
        0,
        0,
        0
            });
            this.durationBox.Location = new Point(87, 32);
            this.durationBox.Maximum = new Decimal(new int[4]
            {
        (int) ushort.MaxValue,
        0,
        0,
        0
            });
            this.durationBox.Name = "durationBox";
            this.durationBox.Size = new Size(80, 20);
            this.durationBox.TabIndex = 8;
            this.AcceptButton = (IButtonControl)this.okButton;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(364, 91);
            this.Controls.Add((Control)this.durationBox);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.sequenceNameLabel);
            this.Controls.Add((Control)this.frameNameBox);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.okButton);
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.Name = nameof(FrameEditWindow);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Maestro Sequencer: Edit Frame";
            this.Shown += new EventHandler(this.FrameEditWindow_Shown);
            this.durationBox.EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

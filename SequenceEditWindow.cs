// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Sequencer.SequenceEditWindow
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace Pololu.Usc.Sequencer
{
    public class SequenceEditWindow : Form
    {
        private IContainer components;
        private Button okButton;
        private Button cancelButton;
        private TextBox sequenceNameBox;
        private Label sequenceNameLabel;

        private SequenceEditWindow()
        {
            this.InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }

        public static DialogResult go(Sequence sequence)
        {
            SequenceEditWindow sequenceEditWindow = new SequenceEditWindow();
            sequenceEditWindow.sequenceNameBox.Text = sequence.name;
            int num = (int)sequenceEditWindow.ShowDialog();
            if (sequenceEditWindow.DialogResult == DialogResult.OK)
                sequence.name = sequenceEditWindow.sequenceNameBox.Text.Trim();
            return sequenceEditWindow.DialogResult;
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

        private void SequenceEditWindow_Load(object sender, EventArgs e)
        {
            this.sequenceNameBox.Focus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SequenceEditWindow));
            this.okButton = new Button();
            this.cancelButton = new Button();
            this.sequenceNameBox = new TextBox();
            this.sequenceNameLabel = new Label();
            this.SuspendLayout();
            this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.okButton.Location = new Point(196, 41);
            this.okButton.Name = "okButton";
            this.okButton.Size = new Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new EventHandler(this.okButton_Click);
            this.cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(277, 41);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
            this.sequenceNameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.sequenceNameBox.Location = new Point(56, 12);
            this.sequenceNameBox.Name = "sequenceNameBox";
            this.sequenceNameBox.Size = new Size(296, 20);
            this.sequenceNameBox.TabIndex = 2;
            this.sequenceNameLabel.AutoSize = true;
            this.sequenceNameLabel.Location = new Point(12, 15);
            this.sequenceNameLabel.Name = "sequenceNameLabel";
            this.sequenceNameLabel.Size = new Size(38, 13);
            this.sequenceNameLabel.TabIndex = 3;
            this.sequenceNameLabel.Text = "Name:";
            this.AcceptButton = (IButtonControl)this.okButton;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(364, 76);
            this.Controls.Add((Control)this.sequenceNameLabel);
            this.Controls.Add((Control)this.sequenceNameBox);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.okButton);
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.Name = nameof(SequenceEditWindow);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Maestro Sequencer: Rename Sequence";
            this.Shown += new EventHandler(this.SequenceEditWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

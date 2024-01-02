// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.AboutBox
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using Pololu.MaestroControlCenter.Properties;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  internal class AboutBox : Form
  {
    private IContainer components;
    private PictureBox logoPictureBox;
    private Label labelVersion;
    private Label labelCompanyName;
    private Button okButton;
    private Label labelCopyright;
    private Label labelProductName;
    private LinkLabel linkLabel1;
    private Label documentationLabel;

    public string AssemblyTitle
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (customAttributes.Length > 0)
        {
          AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
          if (assemblyTitleAttribute.Title != "")
            return assemblyTitleAttribute.Title;
        }
        return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyVersion
    {
      get
      {
        return ((object)Assembly.GetExecutingAssembly().GetName().Version).ToString();
      }
    }

    public string AssemblyDescription
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        else
          return ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        else
          return ((AssemblyProductAttribute)customAttributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        else
          return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        else
          return ((AssemblyCompanyAttribute)customAttributes[0]).Company;
      }
    }

    public AboutBox()
    {
      this.InitializeComponent();
      this.Text = string.Format("About {0}", (object)this.AssemblyTitle);
      this.labelProductName.Text = this.AssemblyProduct;
      this.labelVersion.Text = string.Format("Version {0}", (object)this.AssemblyVersion);
      this.labelCopyright.Text = this.AssemblyCopyright;
      this.labelCompanyName.Text = this.AssemblyCompany;
      this.linkLabel1.Text = "http://www.pololu.com/docs/0J40";
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      //Form1.launchDocs();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.logoPictureBox = new PictureBox();
      this.labelVersion = new Label();
      this.labelCompanyName = new Label();
      this.okButton = new Button();
      this.labelCopyright = new Label();
      this.labelProductName = new Label();
      this.linkLabel1 = new LinkLabel();
      this.documentationLabel = new Label();
      ((ISupportInitialize)this.logoPictureBox).BeginInit();
      this.SuspendLayout();
      this.logoPictureBox.BackColor = Color.White;
      this.logoPictureBox.Image = (Image)Resources.about_box;
      this.logoPictureBox.Location = new Point(7, 8);
      this.logoPictureBox.Name = "logoPictureBox";
      this.logoPictureBox.Padding = new Padding(3);
      this.logoPictureBox.Size = new Size(291, 146);
      this.logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
      this.logoPictureBox.TabIndex = 12;
      this.logoPictureBox.TabStop = false;
      this.labelVersion.AutoSize = true;
      this.labelVersion.Location = new Point(304, 30);
      this.labelVersion.Margin = new Padding(3, 6, 3, 3);
      this.labelVersion.MaximumSize = new Size(0, 17);
      this.labelVersion.Name = "labelVersion";
      this.labelVersion.Size = new Size(42, 13);
      this.labelVersion.TabIndex = 0;
      this.labelVersion.Text = "Version";
      this.labelVersion.TextAlign = ContentAlignment.MiddleLeft;
      this.labelCompanyName.AutoSize = true;
      this.labelCompanyName.Location = new Point(304, 74);
      this.labelCompanyName.Margin = new Padding(3, 6, 3, 3);
      this.labelCompanyName.MaximumSize = new Size(0, 17);
      this.labelCompanyName.Name = "labelCompanyName";
      this.labelCompanyName.Size = new Size(82, 13);
      this.labelCompanyName.TabIndex = 22;
      this.labelCompanyName.Text = "Company Name";
      this.labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
      this.okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.okButton.DialogResult = DialogResult.Cancel;
      this.okButton.Location = new Point(504, 126);
      this.okButton.Name = "okButton";
      this.okButton.Size = new Size(75, 23);
      this.okButton.TabIndex = 24;
      this.okButton.Text = "&OK";
      this.labelCopyright.AutoSize = true;
      this.labelCopyright.Location = new Point(304, 52);
      this.labelCopyright.Margin = new Padding(3, 6, 3, 3);
      this.labelCopyright.MaximumSize = new Size(0, 17);
      this.labelCopyright.Name = "labelCopyright";
      this.labelCopyright.Size = new Size(51, 13);
      this.labelCopyright.TabIndex = 21;
      this.labelCopyright.Text = "Copyright";
      this.labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
      this.labelProductName.AutoSize = true;
      this.labelProductName.Location = new Point(304, 8);
      this.labelProductName.Margin = new Padding(3, 6, 3, 3);
      this.labelProductName.MaximumSize = new Size(0, 17);
      this.labelProductName.Name = "labelProductName";
      this.labelProductName.Size = new Size(75, 13);
      this.labelProductName.TabIndex = 20;
      this.labelProductName.Text = "Product Name";
      this.labelProductName.TextAlign = ContentAlignment.MiddleLeft;
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Location = new Point(320, 111);
      this.linkLabel1.Margin = new Padding(3, 1, 3, 3);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new Size(172, 13);
      this.linkLabel1.TabIndex = 25;
      ((Label)this.linkLabel1).TabStop = true;
      this.linkLabel1.Text = "http://www.pololu.com/docs/0J40";
      this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      this.documentationLabel.AutoSize = true;
      this.documentationLabel.Location = new Point(304, 96);
      this.documentationLabel.Margin = new Padding(3, 3, 3, 1);
      this.documentationLabel.MaximumSize = new Size(0, 17);
      this.documentationLabel.Name = "documentationLabel";
      this.documentationLabel.Size = new Size(118, 13);
      this.documentationLabel.TabIndex = 26;
      this.documentationLabel.Text = "For documentation, see";
      this.documentationLabel.TextAlign = ContentAlignment.MiddleLeft;
      this.AcceptButton = (IButtonControl)this.okButton;
      this.AutoScaleMode = AutoScaleMode.Inherit;
      this.ClientSize = new Size(591, 161);
      this.Controls.Add((Control)this.documentationLabel);
      this.Controls.Add((Control)this.linkLabel1);
      this.Controls.Add((Control)this.logoPictureBox);
      this.Controls.Add((Control)this.labelProductName);
      this.Controls.Add((Control)this.okButton);
      this.Controls.Add((Control)this.labelVersion);
      this.Controls.Add((Control)this.labelCopyright);
      this.Controls.Add((Control)this.labelCompanyName);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBox";
      this.Padding = new Padding(9);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "AboutBox1";
      ((ISupportInitialize)this.logoPictureBox).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

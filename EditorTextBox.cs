// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.EditorTextBox
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  internal class EditorTextBox : RichTextBox
  {
    private static char[] whitespace = new char[4]
    {
      ' ',
      '\t',
      '\r',
      '\n'
    };
    private static List<char> whitespace_list = new List<char>((IEnumerable<char>)EditorTextBox.whitespace);
    private SolidBrush pointerBrush = new SolidBrush(Color.HotPink);
    private Color privateColor0 = Color.Black;
    private Color privateColor1 = Color.Blue;
    private Color privateColor2 = Color.Green;
    private Color privateColor3 = Color.Red;
    private Color privateColor4 = Color.Orange;
    private Color privateCommentColor = Color.Gray;
    private Dictionary<string, int> keywords = new Dictionary<string, int>();
    private List<EditorTextBox.CommentType> commentTypes = new List<EditorTextBox.CommentType>();
    private string lastText = "";
    protected const int commentColorNumber = 5;
    private const byte WM_PAINT = (byte)15;
    private const byte WM_SETREDRAW = (byte)11;
    private LineNumberBox? privateLineNumberBox;
    public bool privatePointerActive;
    public int privatePointerLine;
    public int privatePointerColumn;
    public EventHandler? updateKeywordsCallback;
    private bool painted;
    private bool settingRtf;
    private bool updateRtfWhenEnablingRedraw;
    private bool redrawDisabled;
    private StringBuilder? rtfBuilder;
    private StringBuilder? currentWord;
    private bool currentWordIsComment;
    private EditorTextBox.CommentType? currentCommentType;

    public LineNumberBox? lineNumberBox
    {
      get
      {
        return this.privateLineNumberBox;
      }
      set
      {
        this.privateLineNumberBox = value;
      }
    }

    public Color pointerColor
    {
      get
      {
        return this.pointerBrush.Color;
      }
      set
      {
        this.pointerBrush.Color = value;
      }
    }

    public Color color0
    {
      get
      {
        return this.privateColor0;
      }
      set
      {
        this.privateColor0 = value;
      }
    }

    public Color color1
    {
      get
      {
        return this.privateColor1;
      }
      set
      {
        this.privateColor1 = value;
      }
    }

    public Color color2
    {
      get
      {
        return this.privateColor2;
      }
      set
      {
        this.privateColor2 = value;
      }
    }

    public Color color3
    {
      get
      {
        return this.privateColor3;
      }
      set
      {
        this.privateColor3 = value;
      }
    }

    public Color color4
    {
      get
      {
        return this.privateColor4;
      }
      set
      {
        this.privateColor4 = value;
      }
    }

    public Color commentColor
    {
      get
      {
        return this.privateCommentColor;
      }
      set
      {
        this.privateCommentColor = value;
      }
    }

    public bool pointerActive
    {
      get
      {
        return this.privatePointerActive;
      }
      set
      {
        bool pointerActive = this.pointerActive;
        this.privatePointerActive = value;
        if (pointerActive == value)
          return;
        this.Invalidate();
      }
    }

    public int pointerLine
    {
      get
      {
        return this.privatePointerLine;
      }
      set
      {
        int pointerLine = this.pointerLine;
        this.privatePointerLine = value;
        if (pointerLine == value)
          return;
        this.Invalidate();
      }
    }

    public int pointerColumn
    {
      get
      {
        return this.privatePointerColumn;
      }
      set
      {
        int pointerColumn = this.pointerColumn;
        this.privatePointerColumn = value;
        if (pointerColumn == value)
          return;
        this.Invalidate();
      }
    }

    public void addKeyword(string name, int color)
    {
      if (this.keywords.ContainsKey(name))
        return;
      this.keywords[name.ToLowerInvariant()] = color;
    }

    public void removeAllKeywordsOfColor(int color)
    {
      List<string> list = new List<string>();
      foreach (KeyValuePair<string, int> keyValuePair in this.keywords)
      {
        if (keyValuePair.Value == color)
          list.Add(keyValuePair.Key);
      }
      foreach (string key in list)
        this.keywords.Remove(key);
    }

    public void addCommentType(string start, string stop)
    {
      this.commentTypes.Add(new EditorTextBox.CommentType(start, stop));
    }

    protected override void OnTextChanged(EventArgs e)
    {
      if (!this.settingRtf && !this.redrawDisabled)
        base.OnTextChanged(e);
      this.updateText();
    }

    private void updateText()
    {
      if (!this.redrawDisabled)
        this.updateRtf();
      else
        this.updateRtfWhenEnablingRedraw = true;
      this.lastText = this.Text;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      if (!(this.lastText != this.Text))
        return;
      this.updateText();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      this.painted = true;
    }

    protected void updateRtf()
    {
      if (this.settingRtf)
        return;
      this.settingRtf = true;
      string str = "";
      try
      {
        str = this.Rtf;
      }
      catch (Exception ex)
      {
      }
      if (this.updateKeywordsCallback != null)
        this.updateKeywordsCallback((object)null, (EventArgs)null);
      this.disableRedraw();
      TextBoxScrollPosition p = TextBoxScrolling.saveScrollPosition((TextBoxBase)this);
      this.setRtf();
      TextBoxScrolling.restoreScrollPosition((TextBoxBase)this, p);
      this.enableRedraw();
      this.settingRtf = false;
      if (this.Rtf != str || !this.painted)
        this.Invalidate();
      this.updateLineNumberBox();
      this.updateRtfWhenEnablingRedraw = false;
    }

    protected override void OnVScroll(EventArgs e)
    {
      base.OnVScroll(e);
      this.updateLineNumberBox();
    }

    protected void updateLineNumberBox()
    {
      if (this.lineNumberBox == null)
        return;
      this.lineNumberBox.topY = this.GetPositionFromCharIndex(0).Y;
    }

    public void disableRedraw()
    {
      /*
      this.redrawDisabled = true;
      this.WndProc(ref new Message()
      {
        Msg = 11,
        WParam = (IntPtr) 0,
        HWnd = this.Handle
      });
      */
    }

    public void enableRedraw()
    {
      /*
      this.redrawDisabled = false;
      this.WndProc(ref new Message()
      {
        Msg = 11,
        WParam = (IntPtr)1,
        HWnd = this.Handle
      });
      if (this.settingRtf || !this.updateRtfWhenEnablingRedraw)
        return;
      this.updateRtf();
      */
    }

    protected override void WndProc(ref Message m)
    {
      try
      {
        if (m.Msg == 15)
        {
          base.WndProc(ref m);
          this.customPaint();
        }
        else
          base.WndProc(ref m);
      }
      catch (IndexOutOfRangeException ex)
      {
        Console.WriteLine("Caught an exception in EditorTextBox WndProc:");
        Console.WriteLine(((object)ex).ToString());
      }
    }

    protected void customPaint()
    {
      Graphics graphics = this.CreateGraphics();
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      this.paintPointer(graphics);
    }

    protected void paintPointer(Graphics g)
    {
      if (!this.pointerActive)
        return;
      int num1 = 0;
      int num2 = 0;
      foreach (string str in this.Lines)
      {
        if (num2 + 1 != this.pointerLine)
        {
          ++num2;
          num1 += str.Length + 1;
        }
        else
          break;
      }
      int index = num1 + (this.pointerColumn - 1);
      if (index < 0 || index > this.Text.Length)
        return;
      Point positionFromCharIndex = this.GetPositionFromCharIndex(index);
      PointF pointF1 = new PointF((float)positionFromCharIndex.X, (float)(positionFromCharIndex.Y + this.Font.Height - 6));
      PointF pointF2 = new PointF(pointF1.X + 4f, pointF1.Y + 5.5f);
      PointF pointF3 = new PointF(pointF1.X - 4f, pointF1.Y + 5.5f);
      GraphicsPath path = new GraphicsPath();
      path.AddLine(pointF1, pointF2);
      path.AddLine(pointF2, pointF3);
      path.AddLine(pointF3, pointF1);
      g.FillPath((Brush)this.pointerBrush, path);
    }

    private void parseText()
    {
      this.currentWord = new StringBuilder();
      this.currentWordIsComment = false;
      foreach (char ch in this.Text)
      {
        if (this.currentWordIsComment)
        {
          this.currentWord.Append(ch);
          if (((object)this.currentWord).ToString().EndsWith(this.currentCommentType.stop))
            this.finishCurrentWord();
        }
        else if ((int)ch != 13)
        {
          if ((int)ch == 10)
          {
            this.finishCurrentWord();
            this.rtfBuilder.Append("\n\\par\n");
          }
          else if (EditorTextBox.whitespace_list.Contains(ch))
          {
            this.finishCurrentWord();
            this.rtfBuilder.Append(ch);
          }
          else
          {
            this.currentWord.Append(ch);
            foreach (EditorTextBox.CommentType commentType in this.commentTypes)
            {
              if (((object)this.currentWord).ToString().EndsWith(commentType.start))
              {
                this.currentWord.Remove(this.currentWord.Length - commentType.start.Length, commentType.start.Length);
                this.finishCurrentWord();
                this.currentWord.Append(commentType.start);
                this.currentWordIsComment = true;
                this.currentCommentType = commentType;
              }
            }
          }
        }
      }
      this.finishCurrentWord();
    }

    private void finishCurrentWord()
    {
      int num = -1;
      string str1 = ((object)this.currentWord).ToString();
      if (str1 == "")
      {
        this.currentWord = new StringBuilder();
        this.currentWordIsComment = false;
      }
      else
      {
        str1.Replace("\\", "\\\\");
        string str2 = str1.Replace("{", "\\{").Replace("}", "\\}").Replace("\n", "\n\\par\n");
        if (this.currentWordIsComment)
          num = 5;
        else
          this.keywords.TryGetValue(((object)this.currentWord).ToString().ToLowerInvariant(), out num);
        if (num != -1)
          this.rtfBuilder.Append(string.Concat(new object[4]
          {
            (object) "\\cf",
            (object) num,
            (object) " ",
            (object) str2
          }));
        else
          this.rtfBuilder.Append("\\cf0 " + str2);
        this.currentWord = new StringBuilder();
        this.currentWordIsComment = false;
      }
    }

    private void addColor(Color color)
    {
      this.rtfBuilder.Append("\\red" + (object)color.R + "\\green" + (string)(object)color.G + "\\blue" + (string)(object)color.B + ";");
    }

    protected void setRtf()
    {
      this.rtfBuilder = new StringBuilder("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033");
      this.rtfBuilder.Append("{\\fonttbl{\\f0\\fnil\\fcharset0 " + this.Font.Name + ";}}");
      this.rtfBuilder.Append("{\\colortbl");
      this.addColor(this.color0);
      this.addColor(this.color1);
      this.addColor(this.color2);
      this.addColor(this.color3);
      this.addColor(this.color4);
      this.addColor(this.commentColor);
      this.rtfBuilder.Append("}");
      this.rtfBuilder.Append("\\viewkind4\\uc1\\pard\\f0\\fs" + (object)Math.Round((double)this.Font.SizeInPoints * 2.0));
      if (this.lineNumberBox != null)
        this.rtfBuilder.Append("\\li700 ");
      this.parseText();
      if (System.Type.GetType("Mono.Runtime") != null)
      {
        if (!((object)this.rtfBuilder).ToString().EndsWith("\\par\n"))
          this.rtfBuilder.Append("\\par\n}");
        else
          this.rtfBuilder.Append("}");
      }
      else
        this.rtfBuilder.Append("\\par}");
      this.Rtf = ((object)this.rtfBuilder).ToString();
    }

    private class CommentType
    {
      public string start;
      public string stop;

      public CommentType(string start, string stop)
      {
        this.start = start;
        this.stop = stop;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.TextBoxScrolling
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pololu.MaestroControlCenter
{
  public static class TextBoxScrolling
  {
    public static TextBoxScrollPosition saveScrollPosition(this TextBoxBase t)
    {
      try
      {
        int topChar = !(t is RichTextBox) ? t.GetCharIndexFromPosition(new Point(1, 1 + t.Font.Height)) : t.GetCharIndexFromPosition(new Point(1, 1));
        return new TextBoxScrollPosition(t.SelectionStart, t.SelectionLength, topChar);
      }
      catch (IndexOutOfRangeException ex)
      {
        return new TextBoxScrollPosition(0, 0, 0);
      }
    }

    public static void restoreScrollPosition(this TextBoxBase t, TextBoxScrollPosition p)
    {
      t.SelectionStart = t.Text.Length + 1;
      t.ScrollToCaret();
      t.SelectionStart = p.topChar >= 0 ? p.topChar : 0;
      if (t.Text.Length > t.SelectionStart && (int) t.Text[t.SelectionStart] != 10)
        ++t.SelectionStart;
      t.ScrollToCaret();
      t.SelectionStart = p.selectionStart;
      t.SelectionLength = p.selectionLength;
    }
  }
}

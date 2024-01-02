// Decompiled with JetBrains decompiler
// Type: Pololu.MaestroControlCenter.TextBoxScrollPosition
// Assembly: Pololu Maestro Control Center, Version=1.5.2.0, Culture=neutral, PublicKeyToken=null
// MVID: A72C940A-6248-4FE6-9FE7-62C3134D62FE
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Pololu Maestro Control Center.exe

#nullable disable
namespace Pololu.MaestroControlCenter
{
    public class TextBoxScrollPosition
    {
        private int privateTopChar;
        private int privateSelectionStart;
        private int privateSelectionLength;

        public int topChar => this.privateTopChar;

        public int selectionStart => this.privateSelectionStart;

        public int selectionLength => this.privateSelectionLength;

        public TextBoxScrollPosition(int selectionStart, int selectionLength, int topChar)
        {
            this.privateTopChar = topChar;
            this.privateSelectionLength = selectionLength;
            this.privateSelectionStart = selectionStart;
        }
    }
}

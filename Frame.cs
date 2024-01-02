// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Sequencer.Frame
// Assembly: Sequencer, Version=1.2.4860.23405, Culture=neutral, PublicKeyToken=null
// MVID: B5D3DF98-37EA-4B70-95BF-9B0FEED1ACA2
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Sequencer.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace Pololu.Usc.Sequencer
{
    [Serializable]
    public class Frame
    {
        public string name;
        private ushort[] privateTargets;
        public ushort length_ms;

        [IndexerName("target")]
        public ushort this[int channel]
        {
            get
            {
                return this.privateTargets == null || channel >= this.privateTargets.Length ? (ushort)0 : this.privateTargets[channel];
            }
        }

        public ushort[] targets
        {
            set => this.privateTargets = value;
        }

        public string getTargetsString()
        {
            string targetsString = "";
            for (int index = 0; index < this.privateTargets.Length; ++index)
            {
                if (index != 0)
                    targetsString += " ";
                targetsString += this.privateTargets[index].ToString();
            }
            return targetsString;
        }

        private string getTabSeparatedString()
        {
            string tabSeparatedString = this.name + "\t" + (object)this.length_ms;
            foreach (ushort privateTarget in this.privateTargets)
                tabSeparatedString = tabSeparatedString + "\t" + (object)privateTarget;
            return tabSeparatedString;
        }

        public void setTargetsFromString(string targetsString, byte servoCount)
        {
            ushort[] numArray = new ushort[(int)servoCount];
            string[] strArray = targetsString.Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < strArray.Length; ++index)
            {
                if (index < (int)servoCount)
                {
                    try
                    {
                        numArray[index] = ushort.Parse(strArray[index]);
                    }
                    catch
                    {
                    }
                }
                else
                    break;
            }
            this.targets = numArray;
        }

        public void writeXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Frame));
            writer.WriteAttributeString("name", this.name);
            writer.WriteAttributeString("duration", this.length_ms.ToString());
            writer.WriteString(this.getTargetsString());
            writer.WriteEndElement();
        }

        public static void copyToClipboard(List<Frame> frames)
        {
            if (frames.Count == 0)
                return;
            DataObject data = new DataObject();
            data.SetData((object)frames.ToArray());
            if (frames.Count == 1)
                data.SetData((object)frames[0]);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Frame frame in frames)
                stringBuilder.AppendLine(frame.getTabSeparatedString());
            data.SetText(stringBuilder.ToString());
            Clipboard.SetDataObject((object)data, true);
        }

        public static List<Frame> getFromClipboard()
        {
            if (Clipboard.GetData("Pololu.Usc.Sequencer.Frame[]") is Frame[] data1)
                return new List<Frame>((IEnumerable<Frame>)data1);
            if (Clipboard.GetData("Pololu.Usc.Sequencer.Frame") is Frame data2)
                return new List<Frame>() { data2 };
            if (!Clipboard.ContainsText())
                return (List<Frame>)null;
            List<Frame> fromClipboard = new List<Frame>();
            string text = Clipboard.GetText();
            char[] chArray1 = new char[1] { '\n' };
            foreach (string str in text.Split(chArray1))
            {
                char[] chArray2 = new char[1] { '\t' };
                string[] strArray = str.Split(chArray2);
                if (strArray.Length >= 3)
                {
                    Frame frame = new Frame();
                    frame.name = strArray[0];
                    if (frame.name.Length > 80)
                        frame.name = frame.name.Substring(0, 80);
                    try
                    {
                        frame.length_ms = ushort.Parse(strArray[1]);
                    }
                    catch
                    {
                        frame.length_ms = (ushort)500;
                    }
                    List<ushort> ushortList = new List<ushort>();
                    for (int index = 2; index < strArray.Length; ++index)
                    {
                        try
                        {
                            ushortList.Add(ushort.Parse(strArray[index]));
                        }
                        catch
                        {
                            ushortList.Add((ushort)0);
                        }
                    }
                    frame.targets = ushortList.ToArray();
                    fromClipboard.Add(frame);
                }
            }
            return fromClipboard;
        }
    }
}

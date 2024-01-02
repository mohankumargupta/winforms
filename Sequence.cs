// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Sequencer.Sequence
// Assembly: Sequencer, Version=1.2.4860.23405, Culture=neutral, PublicKeyToken=null
// MVID: B5D3DF98-37EA-4B70-95BF-9B0FEED1ACA2
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Sequencer.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace Pololu.Usc.Sequencer
{
    public class Sequence
    {
        public string name;
        public List<Frame> frames = new List<Frame>();

        public Sequence(string name) => this.name = name;

        public Sequence()
        {
        }

        public static void saveSequencesInRegistry(IList<Sequence> list, RegistryKey parentKey)
        {
            try
            {
                parentKey.DeleteValue("sequences");
            }
            catch
            {
            }
            try
            {
                parentKey.DeleteSubKeyTree("sequences");
            }
            catch
            {
            }
            parentKey.CreateSubKey("sequences");
            RegistryKey registryKey = parentKey.OpenSubKey("sequences", true);
            for (int index1 = 0; index1 < list.Count; ++index1)
            {
                Sequence sequence = list[index1];
                string subkey1 = index1.ToString("d2");
                RegistryKey subKey1 = registryKey.CreateSubKey(subkey1);
                subKey1.SetValue("name", (object)sequence.name, RegistryValueKind.String);
                for (int index2 = 0; index2 < sequence.frames.Count; ++index2)
                {
                    Frame frame = sequence.frames[index2];
                    string subkey2 = index2.ToString("d4");
                    RegistryKey subKey2 = subKey1.CreateSubKey(subkey2);
                    subKey2.SetValue("name", (object)frame.name, RegistryValueKind.String);
                    subKey2.SetValue("duration", (object)(int)frame.length_ms, RegistryValueKind.DWord);
                    subKey2.SetValue("targets", (object)frame.getTargetsString(), RegistryValueKind.String);
                    subKey2.Close();
                }
                subKey1.Close();
            }
            registryKey.Close();
        }

        public static List<Sequence> readSequencesFromRegistry(RegistryKey parentKey, byte servoCount)
        {
            List<Sequence> sequenceList = new List<Sequence>();
            RegistryKey registryKey1 = parentKey.OpenSubKey("sequences");
            if (registryKey1 == null)
                return sequenceList;
            Sequence.FrameKeyNameComparer frameKeyNameComparer = new Sequence.FrameKeyNameComparer();
            if (registryKey1 == null)
                return sequenceList;
            foreach (string subKeyName in registryKey1.GetSubKeyNames())
            {
                RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName);
                if (!(registryKey2.GetValue("name", (object)null) is string name1))
                    name1 = "Sequence " + subKeyName;
                Sequence sequence = new Sequence(name1);
                List<string> stringList = new List<string>((IEnumerable<string>)registryKey2.GetSubKeyNames());
                stringList.Sort((IComparer<string>)frameKeyNameComparer);
                List<Frame> frameList = new List<Frame>(registryKey2.SubKeyCount);
                foreach (string name2 in stringList)
                {
                    RegistryKey registryKey3 = registryKey2.OpenSubKey(name2);
                    if (registryKey3 != null)
                    {
                        Frame frame = new Frame();
                        frame.name = registryKey3.GetValue("name", (object)null) as string;
                        if (frame.name == null)
                            frame.name = "Frame " + name2;
                        int? nullable = registryKey3.GetValue("duration", (object)"") as int?;
                        if (nullable.HasValue)
                            frame.length_ms = (ushort)nullable.Value;
                        frame.setTargetsFromString(registryKey3.GetValue("targets", (object)"") as string, servoCount);
                        frameList.Add(frame);
                    }
                }
                sequence.frames = frameList;
                sequenceList.Add(sequence);
            }
            return sequenceList;
        }

        private string generateScript(
          List<byte> enabled_channels,
          List<List<byte>> needed_channel_lists)
        {
            string script = "";
            Frame frame1 = (Frame)null;
            foreach (Frame frame2 in this.frames)
            {
                List<byte> channels = new List<byte>();
                List<ushort> ushortList = new List<ushort>();
                foreach (byte enabledChannel in enabled_channels)
                {
                    if (frame1 == null || (int)frame2[(int)enabledChannel] != (int)frame1[(int)enabledChannel])
                    {
                        channels.Add(enabledChannel);
                        ushortList.Add(frame2[(int)enabledChannel]);
                    }
                }
                frame1 = frame2;
                if (ushortList.Count != 0)
                {
                    bool flag = false;
                    using (List<List<byte>>.Enumerator enumerator = needed_channel_lists.GetEnumerator())
                    {
                    label_17:
                        while (enumerator.MoveNext())
                        {
                            List<byte> current = enumerator.Current;
                            if (current.Count == channels.Count)
                            {
                                for (int index = 0; index < current.Count; ++index)
                                {
                                    if ((int)current[index] != (int)channels[index])
                                        goto label_17;
                                }
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                        needed_channel_lists.Add(channels);
                }
                script += "  ";
                script = script + (object)frame2.length_ms + " ";
                if (channels.Count == 0)
                {
                    script += "delay";
                }
                else
                {
                    byte num1 = 0;
                    foreach (ushort num2 in ushortList)
                    {
                        if (num1 == (byte)6)
                        {
                            script += "\n  ";
                            num1 = (byte)0;
                        }
                        ++num1;
                        script = script + (object)num2 + " ";
                    }
                    script += Sequence.getFrameSubroutineName(channels);
                }
                script = script + " # " + frame2.name + "\n";
            }
            return script;
        }

        public static string getFrameSubroutineName(List<byte> channels)
        {
            if (channels.Count == 0)
                throw new Exception("getFrameSubroutineName: Expected channels list to be non-empty.");
            string str = "frame_";
            int index1 = 0;
            string frameSubroutineName;
            while (true)
            {
                int index2 = index1 + 1;
                while (index2 < channels.Count && (int)channels[index2 - 1] + 1 == (int)channels[index2])
                    ++index2;
                byte channel1 = channels[index1];
                byte channel2 = channels[index2 - 1];
                if ((int)channel2 == (int)channel1)
                    frameSubroutineName = str + (object)channel1;
                else if ((int)channel2 == (int)channel1 + 1)
                    frameSubroutineName = str + (object)channel1 + "_" + (object)channel2;
                else
                    frameSubroutineName = str + (object)channel1 + ".." + (object)channel2;
                if (index2 != channels.Count)
                {
                    index1 = index2;
                    str = frameSubroutineName + "_";
                }
                else
                    break;
            }
            return frameSubroutineName;
        }

        public static string generateFrameSubroutine(List<byte> channels)
        {
            string str = "sub " + Sequence.getFrameSubroutineName(channels) + "\n";
            for (int index = channels.Count - 1; index >= 0; --index)
                str = str + "  " + channels[index].ToString() + " servo\n";
            return str + "  delay\n" + "  return\n";
        }

        public string generateLoopedScript(List<byte> enabled_channels)
        {
            List<List<byte>> needed_channel_lists = new List<List<byte>>();
            string loopedScript = "# " + this.name + "\nbegin\n" + this.generateScript(enabled_channels, needed_channel_lists) + "repeat\n\n";
            foreach (List<byte> channels in needed_channel_lists)
                loopedScript = loopedScript + Sequence.generateFrameSubroutine(channels) + "\n";
            return loopedScript;
        }

        public string generateSubroutine(
          List<byte> enabled_channels,
          List<List<byte>> needed_channel_lists)
        {
            return "# " + this.name + "\nsub " + new Regex("[^a-z0-9_]", RegexOptions.IgnoreCase).Replace(new Regex("\\s+").Replace(this.name, "_"), "") + "\n" + this.generateScript(enabled_channels, needed_channel_lists) + "  return\n";
        }

        public static string generateSubroutineList(
          List<byte> enabled_channels,
          List<Sequence> sequences)
        {
            List<List<byte>> needed_channel_lists = new List<List<byte>>();
            string subroutineList = "";
            foreach (Sequence sequence in sequences)
                subroutineList += sequence.generateSubroutine(enabled_channels, needed_channel_lists);
            foreach (List<byte> channels in needed_channel_lists)
                subroutineList = subroutineList + "\n" + Sequence.generateFrameSubroutine(channels);
            return subroutineList;
        }

        private class FrameKeyNameComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                try
                {
                    return (int)ushort.Parse(x) - (int)ushort.Parse(y);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}

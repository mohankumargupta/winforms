// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.ConfigurationFile
// Assembly: Usc, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA342C92-C2F7-414D-AF8F-04E408A45146
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Usc.dll

using Pololu.Usc.Sequencer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Pololu.Usc
{
    public static class ConfigurationFile
    {
        public static UscSettings load(StreamReader sr, List<string> warnings)
        {
            XmlReader reader1 = XmlReader.Create((TextReader)sr);
            UscSettings uscSettings = new UscSettings();
            string script = "";
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            reader1.ReadToFollowing("UscSettings");
            ConfigurationFile.readAttributes(reader1, dictionary1);
            XmlReader reader2 = reader1.ReadSubtree();
            if (!dictionary1.ContainsKey("version"))
                warnings.Add("This file has no version number, so it might have been read incorrectly.");
            else if (dictionary1["version"] != "1")
                warnings.Add("Unrecognized settings file version \"" + dictionary1["version"] + "\".");
            reader2.Read();
            while (reader2.Read())
            {
                if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "Channels")
                {
                    ConfigurationFile.readAttributes(reader2, dictionary1);
                    XmlReader reader3 = reader2.ReadSubtree();
                    while (reader3.ReadToFollowing("Channel"))
                    {
                        Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                        ConfigurationFile.readAttributes(reader3, dictionary2);
                        ChannelSetting channelSetting = new ChannelSetting();
                        if (ConfigurationFile.assertKey("name", dictionary2, warnings))
                            channelSetting.name = dictionary2["name"];
                        if (ConfigurationFile.assertKey("mode", dictionary2, warnings))
                        {
                            switch (dictionary2["mode"].ToLowerInvariant())
                            {
                                case "servomultiplied":
                                    channelSetting.mode = ChannelMode.ServoMultiplied;
                                    break;
                                case "servo":
                                    channelSetting.mode = ChannelMode.Servo;
                                    break;
                                case "input":
                                    channelSetting.mode = ChannelMode.Input;
                                    break;
                                case "output":
                                    channelSetting.mode = ChannelMode.Output;
                                    break;
                                default:
                                    warnings.Add("Invalid mode \"" + dictionary2["mode"] + "\".");
                                    break;
                            }
                        }
                        if (ConfigurationFile.assertKey("homemode", dictionary2, warnings))
                        {
                            switch (dictionary2["homemode"].ToLowerInvariant())
                            {
                                case "goto":
                                    channelSetting.homeMode = HomeMode.Goto;
                                    break;
                                case "off":
                                    channelSetting.homeMode = HomeMode.Off;
                                    break;
                                case "ignore":
                                    channelSetting.homeMode = HomeMode.Ignore;
                                    break;
                                default:
                                    warnings.Add("Invalid homemode \"" + dictionary2["homemode"] + "\".");
                                    break;
                            }
                        }
                        if (ConfigurationFile.assertKey("min", dictionary2, warnings))
                            ConfigurationFile.parseU16(dictionary2["min"], ref channelSetting.minimum, "min", warnings);
                        if (ConfigurationFile.assertKey("max", dictionary2, warnings))
                            ConfigurationFile.parseU16(dictionary2["max"], ref channelSetting.maximum, "max", warnings);
                        if (ConfigurationFile.assertKey("home", dictionary2, warnings))
                            ConfigurationFile.parseU16(dictionary2["home"], ref channelSetting.home, "home", warnings);
                        if (ConfigurationFile.assertKey("speed", dictionary2, warnings))
                            ConfigurationFile.parseU16(dictionary2["speed"], ref channelSetting.speed, "speed", warnings);
                        if (ConfigurationFile.assertKey("acceleration", dictionary2, warnings))
                            ConfigurationFile.parseU8(dictionary2["acceleration"], ref channelSetting.acceleration, "acceleration", warnings);
                        if (ConfigurationFile.assertKey("neutral", dictionary2, warnings))
                            ConfigurationFile.parseU16(dictionary2["neutral"], ref channelSetting.neutral, "neutral", warnings);
                        if (ConfigurationFile.assertKey("range", dictionary2, warnings))
                            ConfigurationFile.parseU16(dictionary2["range"], ref channelSetting.range, "range", warnings);
                        uscSettings.channelSettings.Add(channelSetting);
                    }
                    if (reader3.ReadToFollowing("Channel"))
                        warnings.Add("More than " + (object)uscSettings.servoCount + " channel elements were found.  The extra elements have been discarded.");
                }
                else if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "Sequences")
                {
                    XmlReader reader4 = reader2.ReadSubtree();
                    while (reader4.ReadToFollowing("Sequence"))
                    {
                        Sequence sequence = new Sequence();
                        //uscSettings.sequences.Add(sequence);
                        Dictionary<string, string> attributes1 = new Dictionary<string, string>();
                        ConfigurationFile.readAttributes(reader4, attributes1);
                        if (attributes1.ContainsKey("name"))
                        {
                            sequence.name = attributes1["name"];
                        }
                        else
                        {
                            //sequence.name = "Sequence " + (object)uscSettings.sequences.Count;
                            //warnings.Add("No name found for sequence " + sequence.name + ".");
                        }
                        XmlReader reader5 = reader2.ReadSubtree();
                        while (reader5.ReadToFollowing("Frame"))
                        {
                            Frame frame = new Frame();
                            sequence.frames.Add(frame);
                            Dictionary<string, string> attributes2 = new Dictionary<string, string>();
                            ConfigurationFile.readAttributes(reader5, attributes2);
                            if (attributes2.ContainsKey("name"))
                            {
                                frame.name = attributes2["name"];
                            }
                            else
                            {
                                frame.name = "Frame " + (object)sequence.frames.Count;
                                warnings.Add("No name found for " + frame.name + " in sequence \"" + sequence.name + "\".");
                            }
                            if (attributes2.ContainsKey("duration"))
                            {
                                ConfigurationFile.parseU16(attributes2["duration"], ref frame.length_ms, "Duration for frame \"" + frame.name + "\" in sequence \"" + sequence.name + "\".", warnings);
                            }
                            else
                            {
                                frame.name = "Frame " + (object)sequence.frames.Count;
                                warnings.Add("No duration found for frame \"" + frame.name + "\" in sequence \"" + sequence.name + "\".");
                            }
                            frame.setTargetsFromString(reader2.ReadElementContentAsString(), uscSettings.servoCount);
                        }
                    }
                }
                else if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "Script")
                {
                    ConfigurationFile.readAttributes(reader2, dictionary1);
                    script = reader2.ReadElementContentAsString();
                }
                else if (reader2.NodeType == XmlNodeType.Element)
                {
                    try
                    {
                        dictionary1[reader2.Name] = reader2.ReadElementContentAsString();
                    }
                    catch (XmlException ex)
                    {
                        warnings.Add("Unable to parse element \"" + reader2.Name + "\": " + ex.Message);
                    }
                }
            }
            reader2.Close();
            try
            {
                uscSettings.setAndCompileScript(script);
            }
            catch (Exception ex)
            {
                warnings.Add("Error compiling script from XML file: " + ex.Message);
                uscSettings.scriptInconsistent = true;
            }
            if (ConfigurationFile.assertKey("NeverSuspend", dictionary1, warnings))
                ConfigurationFile.parseBool(dictionary1["NeverSuspend"], ref uscSettings.neverSuspend, "NeverSuspend", warnings);
            if (ConfigurationFile.assertKey("SerialMode", dictionary1, warnings))
            {
                /*
                switch (dictionary1["SerialMode"])
                {
                    case "UART_FIXED_BAUD_RATE":
                        uscSettings.serialMode = uscSerialMode.SERIAL_MODE_UART_FIXED_BAUD_RATE;
                        break;
                    case "USB_DUAL_PORT":
                        uscSettings.serialMode = uscSerialMode.SERIAL_MODE_USB_DUAL_PORT;
                        break;
                    case "USB_CHAINED":
                        uscSettings.serialMode = uscSerialMode.SERIAL_MODE_USB_CHAINED;
                        break;
                    default:
                        uscSettings.serialMode = uscSerialMode.SERIAL_MODE_UART_DETECT_BAUD_RATE;
                        break;
                }
                */
            }
            //if (ConfigurationFile.assertKey("FixedBaudRate", dictionary1, warnings))
            //    ConfigurationFile.parseU32(dictionary1["FixedBaudRate"], ref uscSettings.fixedBaudRate, "FixedBaudRate", warnings);
            if (ConfigurationFile.assertKey("SerialTimeout", dictionary1, warnings))
                ConfigurationFile.parseU16(dictionary1["SerialTimeout"], ref uscSettings.serialTimeout, "SerialTimeout", warnings);
            if (ConfigurationFile.assertKey("EnableCrc", dictionary1, warnings))
                ConfigurationFile.parseBool(dictionary1["EnableCrc"], ref uscSettings.enableCrc, "EnableCrc", warnings);
            if (ConfigurationFile.assertKey("SerialDeviceNumber", dictionary1, warnings))
                ConfigurationFile.parseU8(dictionary1["SerialDeviceNumber"], ref uscSettings.serialDeviceNumber, "SerialDeviceNumber", warnings);
            if (ConfigurationFile.assertKey("SerialMiniSscOffset", dictionary1, warnings))
                ConfigurationFile.parseU8(dictionary1["SerialMiniSscOffset"], ref uscSettings.miniSscOffset, "SerialMiniSscOffset", warnings);
            if (ConfigurationFile.assertKey("ScriptDone", dictionary1, warnings))
                ConfigurationFile.parseBool(dictionary1["ScriptDone"], ref uscSettings.scriptDone, "ScriptDone", warnings);
            if (dictionary1.ContainsKey("ServosAvailable"))
                ConfigurationFile.parseU8(dictionary1["ServosAvailable"], ref uscSettings.servosAvailable, "ServosAvailable", warnings);
            if (dictionary1.ContainsKey("ServoPeriod"))
                ConfigurationFile.parseU8(dictionary1["ServoPeriod"], ref uscSettings.servoPeriod, "ServoPeriod", warnings);
            if (dictionary1.ContainsKey("EnablePullups"))
                ConfigurationFile.parseBool(dictionary1["EnablePullups"], ref uscSettings.enablePullups, "EnablePullups", warnings);
            if (dictionary1.ContainsKey("MiniMaestroServoPeriod"))
                ConfigurationFile.parseU32(dictionary1["MiniMaestroServoPeriod"], ref uscSettings.miniMaestroServoPeriod, "MiniMaestroServoPeriod", warnings);
            if (dictionary1.ContainsKey("ServoMultiplier"))
                ConfigurationFile.parseU16(dictionary1["ServoMultiplier"], ref uscSettings.servoMultiplier, "ServoMultiplier", warnings);

            return uscSettings;
        }

        private static void readAttributes(XmlReader reader, Dictionary<string, string> attributes)
        {
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                    attributes[reader.Name] = reader.ReadContentAsString();
            }
            reader.MoveToElement();
        }

        private static void parseBool(
          string input,
          ref bool output,
          string name,
          List<string> warnings)
        {
            bool result;
            if (bool.TryParse(input, out result))
                output = result;
            else
                warnings.Add(name + ": Invalid integer value \"" + input + "\".");
        }

        private static void parseU8(string input, ref byte output, string name, List<string> warnings)
        {
            byte result;
            if (byte.TryParse(input, out result))
                output = result;
            else
                warnings.Add(name + ": Invalid integer value \"" + input + "\".");
        }

        private static void parseU16(
          string input,
          ref ushort output,
          string name,
          List<string> warnings)
        {
            ushort result;
            if (ushort.TryParse(input, out result))
                output = result;
            else
                warnings.Add(name + ": Invalid integer value \"" + input + "\".");
        }

        private static void parseU32(
          string input,
          ref uint output,
          string name,
          List<string> warnings)
        {
            uint result;
            if (uint.TryParse(input, out result))
                output = result;
            else
                warnings.Add(name + ": Invalid integer value \"" + input + "\".");
        }

        private static bool assertKey(
          string key,
          Dictionary<string, string> params_from_xml,
          List<string> warnings)
        {
            if (params_from_xml.ContainsKey(key))
                return true;
            warnings.Add("The " + key + " setting was missing.");
            return false;
        }

        public static void save(UscSettings settings, StreamWriter sw)
        {
            XmlTextWriter writer = new XmlTextWriter((TextWriter)sw);
            writer.Formatting = Formatting.Indented;
            writer.WriteComment("Pololu Maestro servo controller settings file, http://www.pololu.com/catalog/product/1350");
            writer.WriteStartElement("UscSettings");
            writer.WriteAttributeString("version", "1");
            writer.WriteElementString("NeverSuspend", settings.neverSuspend ? "true" : "false");
            //writer.WriteElementString("SerialMode", settings.serialMode.ToString().Replace("SERIAL_MODE_", ""));
            //writer.WriteElementString("FixedBaudRate", settings.fixedBaudRate.ToString());
            writer.WriteElementString("SerialTimeout", settings.serialTimeout.ToString());
            writer.WriteElementString("EnableCrc", settings.enableCrc ? "true" : "false");
            writer.WriteElementString("SerialDeviceNumber", settings.serialDeviceNumber.ToString());
            writer.WriteElementString("SerialMiniSscOffset", settings.miniSscOffset.ToString());
            if (settings.servoCount > (byte)18)
                writer.WriteElementString("EnablePullups", settings.enablePullups ? "true" : "false");
            writer.WriteStartElement("Channels");
            if (settings.servoCount == (byte)6)
            {
                writer.WriteAttributeString("ServosAvailable", settings.servosAvailable.ToString());
                writer.WriteAttributeString("ServoPeriod", settings.servoPeriod.ToString());
            }
            else
            {
                writer.WriteAttributeString("MiniMaestroServoPeriod", settings.miniMaestroServoPeriod.ToString());
                writer.WriteAttributeString("ServoMultiplier", settings.servoMultiplier.ToString());
            }
            writer.WriteComment("Period = " + (settings.periodInMicroseconds / 1000M).ToString() + " ms");
            for (byte index = 0; (int)index < (int)settings.servoCount; ++index)
            {
                ChannelSetting channelSetting = settings.channelSettings[(int)index];
                writer.WriteComment("Channel " + index.ToString());
                writer.WriteStartElement("Channel");
                writer.WriteAttributeString("name", channelSetting.name);
                writer.WriteAttributeString("mode", channelSetting.mode.ToString());
                writer.WriteAttributeString("min", channelSetting.minimum.ToString());
                writer.WriteAttributeString("max", channelSetting.maximum.ToString());
                writer.WriteAttributeString("homemode", channelSetting.homeMode.ToString());
                writer.WriteAttributeString("home", channelSetting.home.ToString());
                writer.WriteAttributeString("speed", channelSetting.speed.ToString());
                writer.WriteAttributeString("acceleration", channelSetting.acceleration.ToString());
                writer.WriteAttributeString("neutral", channelSetting.neutral.ToString());
                writer.WriteAttributeString("range", channelSetting.range.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Sequences");
            /*
            foreach (Sequence sequence in settings.sequences)
            {
                writer.WriteStartElement("Sequence");
                writer.WriteAttributeString("name", sequence.name);
                foreach (Frame frame in sequence.frames)
                    frame.writeXml((XmlWriter)writer);
                writer.WriteEndElement();
            }
            */
            writer.WriteEndElement();
            writer.WriteStartElement("Script");
            writer.WriteAttributeString("ScriptDone", settings.scriptDone ? "true" : "false");
            writer.WriteString(settings.script);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}

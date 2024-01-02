// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Bytecode.BytecodeReader
// Assembly: Bytecode, Version=1.1.4860.23405, Culture=neutral, PublicKeyToken=null
// MVID: A422FA04-51E0-4056-BA68-889B23015D8B
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Bytecode.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

#nullable disable
namespace Pololu.Usc.Bytecode
{
    public static class BytecodeReader
    {
        private static Dictionary<string, Opcode> dictionary;
        private static BytecodeReader.Mode mode;

        private static void initDictionary()
        {
            if (BytecodeReader.dictionary != null)
                return;
            string[] names = Enum.GetNames(typeof(Opcode));
            Opcode[] values = (Opcode[])Enum.GetValues(typeof(Opcode));
            BytecodeReader.dictionary = new Dictionary<string, Opcode>();
            for (int index = 0; index < names.Length; ++index)
                BytecodeReader.dictionary[names[index]] = values[index];
        }

        public static void WriteListing(BytecodeProgram program, string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter((Stream)fileStream);
            int index1 = 0;
            int num1 = 0;
            BytecodeInstruction bytecodeInstruction = (BytecodeInstruction)null;
            if (program.Count != 0)
                bytecodeInstruction = program[index1];
            for (int line = 1; line <= program.getSourceLineCount(); ++line)
            {
                int num2 = 0;
                streamWriter.Write(num1.ToString("X4") + ": ");
                for (; bytecodeInstruction != null && bytecodeInstruction.lineNumber == line; bytecodeInstruction = index1 < program.Count ? program[index1] : (BytecodeInstruction)null)
                {
                    foreach (byte num3 in bytecodeInstruction.toByteList())
                    {
                        streamWriter.Write(num3.ToString("X2"));
                        ++num1;
                        num2 += 2;
                    }
                    ++index1;
                }
                for (int index2 = 0; index2 < 20 - num2; ++index2)
                    streamWriter.Write(" ");
                streamWriter.Write(" -- ");
                streamWriter.WriteLine(program.getSourceLine(line));
            }
            streamWriter.WriteLine("");
            streamWriter.WriteLine("Subroutines:");
            streamWriter.WriteLine("Hex Decimal Address Name");
            string[] strArray = new string[128];
            foreach (KeyValuePair<string, ushort> subroutineAddress1 in program.subroutineAddresses)
            {
                string key = subroutineAddress1.Key;
                if (program.subroutineCommands[key] != (byte)54)
                {
                    byte index3 = (byte)((uint)program.subroutineCommands[key] - 128U);
                    ushort subroutineAddress2 = program.subroutineAddresses[key];
                    strArray[(int)index3] = index3.ToString("X2") + "  " + index3.ToString("D3") + "     " + subroutineAddress2.ToString("X4") + "    " + key;
                }
            }
            for (int index4 = 0; index4 < strArray.Length && strArray[index4] != null; ++index4)
                streamWriter.WriteLine(strArray[index4]);
            foreach (KeyValuePair<string, ushort> subroutineAddress3 in program.subroutineAddresses)
            {
                string key = subroutineAddress3.Key;
                if (program.subroutineCommands[key] == (byte)54)
                {
                    ushort subroutineAddress4 = program.subroutineAddresses[key];
                    streamWriter.WriteLine("--  ---     " + subroutineAddress4.ToString("X4") + "    " + key);
                }
            }
            streamWriter.Close();
            fileStream.Close();
        }

        public static BytecodeProgram Read(string program, bool isMiniMaestro)
        {
            BytecodeReader.initDictionary();
            BytecodeProgram bytecode_program = new BytecodeProgram();
            BytecodeReader.mode = BytecodeReader.Mode.NORMAL;
            if (program == null)
                program = "";
            string[] strArray = program.Split(new string[2]
            {
        "\r\n",
        "\n"
            }, StringSplitOptions.None);
            for (int line_number = 1; line_number <= strArray.Length; ++line_number)
            {
                string str1 = strArray[line_number - 1];
                bytecode_program.addSourceLine(str1);
                int column_number = 1;
                string str2 = Regex.Replace(str1, "#.*", "");
                char[] chArray = new char[2] { ' ', '\t' };
                foreach (string str3 in str2.Split(chArray))
                {
                    if (str3 == "")
                    {
                        ++column_number;
                    }
                    else
                    {
                        string upperInvariant = str3.ToUpperInvariant();
                        switch (BytecodeReader.mode)
                        {
                            case BytecodeReader.Mode.NORMAL:
                                BytecodeReader.parseString(upperInvariant, bytecode_program, "script", line_number, column_number, isMiniMaestro);
                                break;
                            case BytecodeReader.Mode.GOTO:
                                BytecodeReader.parseGoto(upperInvariant, bytecode_program, "script", line_number, column_number);
                                break;
                            case BytecodeReader.Mode.SUBROUTINE:
                                BytecodeReader.parseSubroutine(upperInvariant, bytecode_program, "script", line_number, column_number);
                                break;
                        }
                        column_number += upperInvariant.Length + 1;
                    }
                }
            }
            if (bytecode_program.blockIsOpen)
            {
                string currentBlockStartLabel = bytecode_program.getCurrentBlockStartLabel();
                bytecode_program.findLabelInstruction(currentBlockStartLabel).error("BEGIN block was never closed.");
            }
            bytecode_program.completeLiterals();
            bytecode_program.completeCalls(isMiniMaestro);
            bytecode_program.completeJumps();
            return bytecode_program;
        }

        private static void parseGoto(
          string s,
          BytecodeProgram bytecode_program,
          string filename,
          int line_number,
          int column_number)
        {
            bytecode_program.addInstruction(BytecodeInstruction.newJumpToLabel("USER_" + s, filename, line_number, column_number));
            BytecodeReader.mode = BytecodeReader.Mode.NORMAL;
        }

        private static void parseSubroutine(
          string s,
          BytecodeProgram bytecode_program,
          string filename,
          int line_number,
          int column_number)
        {
            if (BytecodeReader.looksLikeLiteral(s))
                throw new Exception("The name " + s + " is not valid as a subroutine name (it looks like a number).");
            if (BytecodeReader.dictionary.ContainsKey(s))
                throw new Exception("The name " + s + " is not valid as a subroutine name (it is a built-in command).");
            foreach (string name in Enum.GetNames(typeof(Keyword)))
            {
                if (name == s)
                    throw new Exception("The name " + s + " is not valid as a subroutine name (it is a keyword).");
            }
            bytecode_program.addInstruction(BytecodeInstruction.newSubroutine(s, filename, line_number, column_number));
            BytecodeReader.mode = BytecodeReader.Mode.NORMAL;
        }

        private static bool looksLikeLiteral(string s)
        {
            return Regex.Match(s, "^-?[0-9.]+$").Success || Regex.Match(s, "^0[xX][0-9a-fA-F.]+$").Success;
        }

        private static void parseString(
          string s,
          BytecodeProgram bytecode_program,
          string filename,
          int line_number,
          int column_number,
          bool isMiniMaestro)
        {
            try
            {
                if (BytecodeReader.looksLikeLiteral(s))
                {
                    Decimal num;
                    if (s.StartsWith("0X"))
                    {
                        num = (Decimal)long.Parse(s.Substring(2), NumberStyles.HexNumber);
                        if (num > 65535M || num < 0M)
                            throw new Exception("Value " + s + " is not in the allowed range of " + (object)(ushort)0 + " to " + (object)ushort.MaxValue + ".");
                        if ((Decimal)(ushort)num != num)
                            throw new Exception("Value " + s + " must be an integer.");
                    }
                    else
                    {
                        num = Decimal.Parse(s);
                        if (num > 32767M || num < -32768M)
                            throw new Exception("Value " + s + " is not in the allowed range of " + (object)short.MinValue + " to " + (object)short.MaxValue + ".");
                        if ((Decimal)(short)num != num)
                            throw new Exception("Value " + s + " must be an integer.");
                    }
                    int literal = (int)(short)(long)(num % 65535M);
                    bytecode_program.addLiteral(literal, filename, line_number, column_number, isMiniMaestro);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing " + s + ": " + ex.ToString());
            }
            if (s == Keyword.GOTO.ToString())
                BytecodeReader.mode = BytecodeReader.Mode.GOTO;
            else if (s == Keyword.SUB.ToString())
            {
                BytecodeReader.mode = BytecodeReader.Mode.SUBROUTINE;
            }
            else
            {
                Match match = Regex.Match(s, "(.*):$");
                if (match.Success)
                    bytecode_program.addInstruction(BytecodeInstruction.newLabel("USER_" + match.Groups[1].ToString(), filename, line_number, column_number));
                else if (s == Keyword.BEGIN.ToString())
                    bytecode_program.openBlock(BlockType.BEGIN, filename, line_number, column_number);
                else if (s == Keyword.WHILE.ToString())
                {
                    if (bytecode_program.getCurrentBlockType() != BlockType.BEGIN)
                        throw new Exception("WHILE must be inside a BEGIN...REPEAT block");
                    bytecode_program.addInstruction(BytecodeInstruction.newConditionalJumpToLabel(bytecode_program.getCurrentBlockEndLabel(), filename, line_number, column_number));
                }
                else if (s == Keyword.REPEAT.ToString())
                {
                    try
                    {
                        if (bytecode_program.getCurrentBlockType() != BlockType.BEGIN)
                            throw new Exception("REPEAT must end a BEGIN...REPEAT block");
                        bytecode_program.addInstruction(BytecodeInstruction.newJumpToLabel(bytecode_program.getCurrentBlockStartLabel(), filename, line_number, column_number));
                        bytecode_program.closeBlock(filename, line_number, column_number);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new Exception(filename + ":" + (object)line_number + ":" + (object)column_number + ": Found REPEAT without a corresponding BEGIN");
                    }
                }
                else if (s == Keyword.IF.ToString())
                {
                    bytecode_program.openBlock(BlockType.IF, filename, line_number, column_number);
                    bytecode_program.addInstruction(BytecodeInstruction.newConditionalJumpToLabel(bytecode_program.getCurrentBlockEndLabel(), filename, line_number, column_number));
                }
                else if (s == Keyword.ENDIF.ToString())
                {
                    try
                    {
                        if (bytecode_program.getCurrentBlockType() != BlockType.IF && bytecode_program.getCurrentBlockType() != BlockType.ELSE)
                            throw new Exception("ENDIF must end an IF...ENDIF or an IF...ELSE...ENDIF block.");
                        bytecode_program.closeBlock(filename, line_number, column_number);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new Exception(filename + ":" + (object)line_number + ":" + (object)column_number + ": Found ENDIF without a corresponding IF");
                    }
                }
                else if (s == Keyword.ELSE.ToString())
                {
                    try
                    {
                        if (bytecode_program.getCurrentBlockType() != BlockType.IF)
                            throw new Exception("ELSE must be part of an IF...ELSE...ENDIF block.");
                        bytecode_program.addInstruction(BytecodeInstruction.newJumpToLabel(bytecode_program.getNextBlockEndLabel(), filename, line_number, column_number));
                        bytecode_program.closeBlock(filename, line_number, column_number);
                        bytecode_program.openBlock(BlockType.ELSE, filename, line_number, column_number);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new Exception(filename + ":" + (object)line_number + ":" + (object)column_number + ": Found ELSE without a corresponding IF");
                    }
                }
                else
                {
                    try
                    {
                        Opcode op = BytecodeReader.dictionary[s];
                        switch (op)
                        {
                            case Opcode.LITERAL:
                            case Opcode.LITERAL8:
                            case Opcode.LITERAL_N:
                            case Opcode.LITERAL8_N:
                                throw new Exception(filename + ":" + (object)line_number + ":" + (object)column_number + ": Literal commands may not be used directly in a program.  Integers should be entered directly.");
                            case Opcode.JUMP:
                            case Opcode.JUMP_Z:
                                throw new Exception(filename + ":" + (object)line_number + ":" + (object)column_number + ": Jumps may not be used directly in a program.");
                            default:
                                if (!isMiniMaestro && (byte)op >= (byte)50)
                                    throw new Exception(filename + ":" + (object)line_number + ":" + (object)column_number + ": " + op.ToString() + " is only available on the Mini Maestro 12, 18, and 24.");
                                bytecode_program.addInstruction(new BytecodeInstruction(op, filename, line_number, column_number));
                                break;
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        bytecode_program.addInstruction(BytecodeInstruction.newCall(s, filename, line_number, column_number));
                    }
                }
            }
        }

        private enum Mode
        {
            NORMAL,
            GOTO,
            SUBROUTINE,
        }
    }
}

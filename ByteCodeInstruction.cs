// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Bytecode.BytecodeInstruction
// Assembly: Bytecode, Version=1.1.4860.23405, Culture=neutral, PublicKeyToken=null
// MVID: A422FA04-51E0-4056-BA68-889B23015D8B
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Bytecode.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Pololu.Usc.Bytecode
{
    public class BytecodeInstruction
    {
        private string privateFilename;
        private int privateLineNumber;
        private int privateColumnNumber;
        private Opcode privateOpcode;
        private List<int> privateLiteralArguments = new List<int>();
        private bool privateIsLabel;
        private bool privateIsJumpToLabel;
        private string privateLabelName;
        private bool privateIsSubroutine;
        private bool privateIsCall;

        public string filename => this.privateFilename;

        public int lineNumber => this.privateLineNumber;

        public int columnNumber => this.privateColumnNumber;

        public Opcode opcode => this.privateOpcode;

        public List<int> literalArguments => this.privateLiteralArguments;

        public void addLiteralArgument(int value, bool isMiniMaestro)
        {
            this.privateLiteralArguments.Add(value);
            if (!isMiniMaestro && this.privateLiteralArguments.Count > 32)
                throw new Exception("Too many literals (> 32) in a row: this will overflow the stack.");
            if (this.privateLiteralArguments.Count > 126)
                throw new Exception("Too many literals (> 126) in a row: this will overflow the stack.");
        }

        public void setOpcode(byte value)
        {
            if (this.opcode != Opcode.QUIT)
                throw new Exception("The opcode has already been set.");
            this.privateOpcode = (Opcode)value;
        }

        public bool isLabel => this.privateIsLabel;

        public bool isJumpToLabel => this.privateIsJumpToLabel;

        public string labelName => this.privateLabelName;

        public bool isSubroutine => this.privateIsSubroutine;

        public bool isCall => this.privateIsCall;

        public BytecodeInstruction(Opcode op, string filename, int lineNumber, int columnNumber)
        {
            this.privateOpcode = op;
            this.privateFilename = filename;
            this.privateLineNumber = lineNumber;
            this.privateColumnNumber = columnNumber;
        }

        public BytecodeInstruction(
          Opcode op,
          int literalArgument,
          string filename,
          int lineNumber,
          int columnNumber)
        {
            this.privateOpcode = op;
            this.privateLiteralArguments.Add(literalArgument);
            this.privateFilename = filename;
            this.privateLineNumber = lineNumber;
            this.privateColumnNumber = columnNumber;
        }

        public List<byte> toByteList()
        {
            List<byte> byteList = new List<byte>();
            if (this.isLabel || this.isSubroutine)
                return byteList;
            byteList.Add((byte)this.opcode);
            if (this.opcode == Opcode.LITERAL || this.opcode == Opcode.JUMP || this.opcode == Opcode.JUMP_Z || this.opcode == Opcode.CALL)
            {
                if (this.literalArguments.Count == 0)
                {
                    byteList.Add((byte)0);
                    byteList.Add((byte)0);
                }
                else
                {
                    byteList.Add((byte)((uint)(ushort)this.literalArguments[0] % 256U));
                    byteList.Add((byte)((uint)(ushort)this.literalArguments[0] / 256U));
                }
            }
            else if (this.opcode == Opcode.LITERAL8)
                byteList.Add((byte)this.literalArguments[0]);
            else if (this.opcode == Opcode.LITERAL_N)
            {
                byteList.Add((byte)(this.literalArguments.Count * 2));
                foreach (int literalArgument in this.literalArguments)
                {
                    byteList.Add((byte)((uint)(ushort)literalArgument % 256U));
                    byteList.Add((byte)((uint)(ushort)literalArgument / 256U));
                }
            }
            else if (this.opcode == Opcode.LITERAL8_N)
            {
                byteList.Add((byte)this.literalArguments.Count);
                foreach (int literalArgument in this.literalArguments)
                    byteList.Add((byte)literalArgument);
            }
            return byteList;
        }

        public void error(string msg)
        {
            throw new Exception(this.filename + ":" + (object)this.lineNumber + ":" + (object)this.columnNumber + ": " + msg);
        }

        public static BytecodeInstruction newSubroutine(
          string name,
          string filename,
          int column_number,
          int line_number)
        {
            return new BytecodeInstruction(Opcode.QUIT, filename, column_number, line_number)
            {
                privateIsSubroutine = true,
                privateLabelName = name
            };
        }

        public static BytecodeInstruction newCall(
          string name,
          string filename,
          int column_number,
          int line_number)
        {
            return new BytecodeInstruction(Opcode.QUIT, filename, column_number, line_number)
            {
                privateIsCall = true,
                privateLabelName = name
            };
        }

        public static BytecodeInstruction newLabel(
          string name,
          string filename,
          int column_number,
          int line_number)
        {
            return new BytecodeInstruction(Opcode.QUIT, filename, column_number, line_number)
            {
                privateIsLabel = true,
                privateLabelName = name
            };
        }

        public static BytecodeInstruction newJumpToLabel(
          string name,
          string filename,
          int column_number,
          int line_number)
        {
            return new BytecodeInstruction(Opcode.JUMP, filename, column_number, line_number)
            {
                privateIsJumpToLabel = true,
                privateLabelName = name
            };
        }

        public static BytecodeInstruction newConditionalJumpToLabel(
          string name,
          string filename,
          int column_number,
          int line_number)
        {
            return new BytecodeInstruction(Opcode.JUMP_Z, filename, column_number, line_number)
            {
                privateIsJumpToLabel = true,
                privateLabelName = name
            };
        }

        public void completeLiterals()
        {
            if (this.opcode != Opcode.LITERAL)
                return;
            bool flag = false;
            foreach (int literalArgument in this.literalArguments)
            {
                if (literalArgument > (int)byte.MaxValue || literalArgument < 0)
                    flag = true;
            }
            if (flag && this.literalArguments.Count > 1)
                this.privateOpcode = Opcode.LITERAL_N;
            else if (flag && this.literalArguments.Count == 1)
                this.privateOpcode = Opcode.LITERAL;
            else if (this.literalArguments.Count > 1)
                this.privateOpcode = Opcode.LITERAL8_N;
            else
                this.privateOpcode = Opcode.LITERAL8;
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Bytecode.BytecodeProgram
// Assembly: Bytecode, Version=1.1.4860.23405, Culture=neutral, PublicKeyToken=null
// MVID: A422FA04-51E0-4056-BA68-889B23015D8B
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Bytecode.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Pololu.Usc.Bytecode
{
    public class BytecodeProgram
    {
        private const ushort CRC16_POLY = 40961;
        private List<string> privateSourceLines = new List<string>();
        private List<BytecodeInstruction> instructionList = new List<BytecodeInstruction>();
        private int maxBlock;
        private Stack<int> openBlocks = new Stack<int>();
        private Stack<BlockType> openBlockTypes = new Stack<BlockType>();
        public Dictionary<string, ushort> subroutineAddresses = new Dictionary<string, ushort>();
        public Dictionary<string, byte> subroutineCommands = new Dictionary<string, byte>();

        public string getSourceLine(int line) => this.privateSourceLines[line - 1];

        public void addSourceLine(string line) => this.privateSourceLines.Add(line);

        public int getSourceLineCount() => this.privateSourceLines.Count;

        public BytecodeInstruction this[int index] => this.instructionList[index];

        public int Count => this.instructionList.Count;

        internal void addInstruction(BytecodeInstruction instruction)
        {
            this.instructionList.Add(instruction);
        }

        internal void addLiteral(
          int literal,
          string filename,
          int lineNumber,
          int columnNumber,
          bool isMiniMaestro)
        {
            if (this.instructionList.Count == 0 || this.instructionList[this.instructionList.Count - 1].opcode != Opcode.LITERAL)
                this.addInstruction(new BytecodeInstruction(Opcode.LITERAL, filename, lineNumber, columnNumber));
            this.instructionList[this.instructionList.Count - 1].addLiteralArgument(literal, isMiniMaestro);
        }

        public List<byte> getByteList()
        {
            List<byte> byteList = new List<byte>();
            foreach (BytecodeInstruction instruction in this.instructionList)
                byteList.AddRange((IEnumerable<byte>)instruction.toByteList());
            return byteList;
        }

        internal void openBlock(
          BlockType blocktype,
          string filename,
          int line_number,
          int column_number)
        {
            this.addInstruction(BytecodeInstruction.newLabel("block_start_" + this.maxBlock.ToString(), filename, line_number, column_number));
            this.openBlocks.Push(this.maxBlock);
            this.openBlockTypes.Push(blocktype);
            ++this.maxBlock;
        }

        internal BlockType getCurrentBlockType() => this.openBlockTypes.Peek();

        internal string getCurrentBlockStartLabel()
        {
            return "block_start_" + this.openBlocks.Peek().ToString();
        }

        internal string getCurrentBlockEndLabel() => "block_end_" + this.openBlocks.Peek().ToString();

        internal string getNextBlockEndLabel() => "block_end_" + this.maxBlock.ToString();

        public int findLabelIndex(string name)
        {
            for (int index = 0; index < this.instructionList.Count; ++index)
            {
                if (this.instructionList[index].isLabel && this.instructionList[index].labelName == name)
                    return index;
            }
            throw new Exception("Label not found.");
        }

        public BytecodeInstruction findLabelInstruction(string name)
        {
            return this.instructionList[this.findLabelIndex(name)];
        }

        internal void closeBlock(string filename, int line_number, int column_number)
        {
            this.addInstruction(BytecodeInstruction.newLabel("block_end_" + this.openBlocks.Pop().ToString(), filename, line_number, column_number));
            int num = (int)this.openBlockTypes.Pop();
        }

        internal bool blockIsOpen => this.openBlocks.Count > 0;

        internal void completeJumps()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int num = 0;
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                if (instruction.isLabel)
                {
                    if (dictionary.ContainsKey(instruction.labelName))
                        instruction.error("The label " + instruction.labelName + " has already been used.");
                    dictionary[instruction.labelName] = num;
                }
                num += instruction.toByteList().Count;
            }
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                try
                {
                    if (instruction.isJumpToLabel)
                        instruction.addLiteralArgument(dictionary[instruction.labelName], false);
                }
                catch (KeyNotFoundException ex)
                {
                    instruction.error("The label " + instruction.labelName + " was not found.");
                }
            }
        }

        internal void completeCalls(bool isMiniMaestro)
        {
            uint num1 = 128;
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                if (instruction.isSubroutine)
                {
                    if (this.subroutineCommands.ContainsKey(instruction.labelName))
                        instruction.error("The subroutine " + instruction.labelName + " has already been defined.");
                    this.subroutineCommands[instruction.labelName] = num1 < 256U ? (byte)num1 : (byte)54;
                    ++num1;
                    if (num1 > (uint)byte.MaxValue && !isMiniMaestro)
                        instruction.error("Too many subroutines.  The limit for the Micro Maestro is 128.");
                }
            }
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                try
                {
                    if (instruction.isCall)
                        instruction.setOpcode(this.subroutineCommands[instruction.labelName]);
                }
                catch (KeyNotFoundException ex)
                {
                    instruction.error("Did not understand '" + instruction.labelName + "'");
                }
            }
            int num2 = 0;
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                if (instruction.isSubroutine)
                    this.subroutineAddresses[instruction.labelName] = (ushort)num2;
                num2 += instruction.toByteList().Count;
            }
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                if (instruction.opcode == Opcode.CALL)
                    instruction.literalArguments.Add((int)this.subroutineAddresses[instruction.labelName]);
            }
        }

        internal void completeLiterals()
        {
            foreach (BytecodeInstruction instruction in this.instructionList)
                instruction.completeLiterals();
        }

        public BytecodeInstruction getInstructionAt(ushort program_counter)
        {
            int num = 0;
            foreach (BytecodeInstruction instruction in this.instructionList)
            {
                List<byte> byteList = instruction.toByteList();
                if (num >= (int)program_counter && byteList.Count != 0)
                    return instruction;
                num += byteList.Count;
            }
            return (BytecodeInstruction)null;
        }

        public ushort getCRC()
        {
            List<byte> message = new List<byte>();
            ushort[] numArray = new ushort[128];
            foreach (string key in this.subroutineCommands.Keys)
            {
                if (this.subroutineCommands[key] != (byte)54)
                    numArray[(int)this.subroutineCommands[key] - 128] = this.subroutineAddresses[key];
            }
            foreach (ushort num in numArray)
            {
                message.Add((byte)((uint)num & (uint)byte.MaxValue));
                message.Add((byte)((uint)num >> 8));
            }
            message.AddRange((IEnumerable<byte>)this.getByteList());
            return BytecodeProgram.CRC(message);
        }

        private static ushort oneByteCRC(byte v)
        {
            ushort num = (ushort)v;
            for (int index = 0; index < 8; ++index)
            {
                if (((int)num & 1) == 1)
                    num = (ushort)((int)num >> 1 ^ 40961);
                else
                    num >>= 1;
            }
            return num;
        }

        private static ushort CRC(List<byte> message)
        {
            ushort num = 0;
            for (ushort index = 0; (int)index < message.Count; ++index)
                num = (ushort)((uint)num >> 8 ^ (uint)BytecodeProgram.oneByteCRC((byte)((uint)(byte)num ^ (uint)message[(int)index])));
            return num;
        }
    }
}

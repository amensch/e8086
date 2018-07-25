using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class GRP5 : GroupInstruction
    {
        public GRP5(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void LoadInstructionList()
        {
            // REG 000: INC mem-16
            // REG 001: DEC mem-16
            // REG 002: CALL reg/mem-16
            // REG 003: CALL mem-16
            // REG 004: JMP reg/mem-16
            // REG 005: JMP mem-16
            // REG 006: PUSH mem-16
            // REG 007: undefined

            instructions.Add(0x00, new INC_RegMem(OpCode, EU, Bus));
            instructions.Add(0x01, new DEC_RegMem(OpCode, EU, Bus));
            instructions.Add(0x02, new CALL_RegMem(OpCode, EU, Bus));
            instructions.Add(0x03, new CALL_Mem(OpCode, EU, Bus));
            instructions.Add(0x04, new JMP_RegMem(OpCode, EU, Bus));
            instructions.Add(0x05, new JMP_Mem(OpCode, EU, Bus));
            instructions.Add(0x06, new PUSH_Mem(OpCode, EU, Bus));
        }
    }

    internal class CALL_RegMem : TwoByteInstruction
    {
        public CALL_RegMem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override void ExecuteInstruction()
        {
            // CALL reg/mem-16 (intrasegment)
            int dest = GetDestinationData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            Push(Bus.IP);
            Bus.IP = (ushort)dest;
        }
    }

    internal class CALL_Mem : TwoByteInstruction
    {
        public CALL_Mem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // CALL reg/mem-16 (intrasegment)
            int dest = GetDestinationData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            Push(Bus.CS);
            Push(Bus.IP);
            Bus.IP = Bus.GetWord(dest);
            Bus.CS = Bus.GetWord(dest + 2);
        }
    }

    internal class JMP_RegMem : TwoByteInstruction
    {
        public JMP_RegMem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // JMP reg/mem-16 (intrasegment)
            int dest = GetDestinationData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            Bus.IP = (ushort)dest;
        }
    }

    internal class JMP_Mem : TwoByteInstruction
    {
        public JMP_Mem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // JMP mem-16 (intrasegment)
            int dest = GetDestinationData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            Bus.IP = Bus.GetWord(dest);
            Bus.CS = Bus.GetWord(dest + 2);
        }
    }

    internal class PUSH_Mem : TwoByteInstruction
    {
        public PUSH_Mem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // PUSH mem-16
            int dest = GetDestinationData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            Push((ushort)dest);
        }
    }
}

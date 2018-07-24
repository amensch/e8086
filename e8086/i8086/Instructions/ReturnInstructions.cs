using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class RET_ImmediateWord : Instruction
    {
        public RET_ImmediateWord(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort operand = GetImmediateWord();
            Bus.IP = Pop();
            EU.Registers.SP += operand;
        }
    }

    internal class RET : Instruction
    {
        public RET(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP = Pop();
        }
    }

    internal class RETF_ImmediateWord : Instruction
    {
        public RETF_ImmediateWord(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort operand = GetImmediateWord();
            Bus.IP = Pop();
            Bus.CS = Pop();
            EU.Registers.SP += operand;
        }
    }

    internal class RETF : Instruction
    {
        public RETF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP = Pop();
            Bus.CS = Pop();
        }
    }

    internal class IRET : Instruction
    {
        public IRET(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP = Pop();
            Bus.CS = Pop();
            EU.CondReg.Value = Pop();
        }
    }
}

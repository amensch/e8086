using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class RET_ImmediateWord : Instruction
    {
        public RET_ImmediateWord(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort operand = GetImmediateWord();
            Bus.IP = Pop();
            EU.Registers.SP += operand;
        }
    }

    public class RET : Instruction
    {
        public RET(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP = Pop();
        }
    }

    public class RETF_ImmediateWord : Instruction
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

    public class RETF : Instruction
    {
        public RETF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP = Pop();
            Bus.CS = Pop();
        }
    }

    public class IRET : Instruction
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

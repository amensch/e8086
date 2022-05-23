using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
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

        public override long Clocks()
        {
            return 12;  // 0xc2 RET intrasegment, immediate
        }
    }

    internal class RET : Instruction
    {
        public RET(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP = Pop();
        }
        public override long Clocks()
        {
            return 8;  // 0xc3 RET intrasegment
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

        public override long Clocks()
        {
            return 17; // 0xca RET intersegment, immediate
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
        public override long Clocks()
        {
            return 18; // 0xcb RET intersegment, immediate
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

        public override long Clocks()
        {
            return 24;
        }
    }
}

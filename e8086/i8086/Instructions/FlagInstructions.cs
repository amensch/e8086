using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    abstract internal class FlagInstruction : Instruction
    {
        public FlagInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        public override long Clocks()
        {
            return 2;
        }
    }

    internal class CMC : FlagInstruction
    {
        public CMC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CMC - complement carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = !EU.CondReg.CarryFlag;
        }
    }

    internal class CLC : FlagInstruction
    {
        public CLC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLC - clear carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = false;
        }
    }

    internal class STC : FlagInstruction
    {
        public STC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STC - set carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = true;
        }
    }

    internal class CLI : FlagInstruction
    {
        public CLI(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLI - clear interrupt flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.InterruptEnable = false;
        }
    }

    internal class STI : FlagInstruction
    {
        public STI(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STI - set interrupt flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.InterruptEnable = true;
        }
    }

    internal class CLD : FlagInstruction
    {
        public CLD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLD - clear direction flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.DirectionFlag = false;
        }
    }

    internal class STD : FlagInstruction
    {
        public STD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STD - set direction flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.DirectionFlag = true;
        }
    }
}

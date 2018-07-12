using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class CMC : Instruction
    {
        public CMC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CMC - complement carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = !EU.CondReg.CarryFlag;
        }
    }

    public class CLC : Instruction
    {
        public CLC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLC - clear carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = false;
        }
    }

    public class STC : Instruction
    {
        public STC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STC - set carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = true;
        }
    }

    public class CLI : Instruction
    {
        public CLI(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLI - clear interrupt flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.InterruptEnable = false;
        }
    }

    public class STI : Instruction
    {
        public STI(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STI - set interrupt flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.InterruptEnable = true;
        }
    }

    public class CLD : Instruction
    {
        public CLD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLD - clear direction flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.DirectionFlag = false;
        }
    }

    public class STD : Instruction
    {
        public STD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STD - set direction flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.DirectionFlag = true;
        }
    }
}

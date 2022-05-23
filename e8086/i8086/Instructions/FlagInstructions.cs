
namespace KDS.e8086.Instructions
{
    internal class CMC : Instruction
    {
        public CMC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CMC - complement carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = !EU.CondReg.CarryFlag;
        }
    }

    internal class CLC : Instruction
    {
        public CLC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLC - clear carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = false;
        }
    }

    internal class STC : Instruction
    {
        public STC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STC - set carry flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.CarryFlag = true;
        }
    }

    internal class CLI : Instruction
    {
        public CLI(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLI - clear interrupt flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.InterruptEnable = false;
        }
    }

    internal class STI : Instruction
    {
        public STI(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STI - set interrupt flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.InterruptEnable = true;
        }
    }

    internal class CLD : Instruction
    {
        public CLD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CLD - clear direction flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.DirectionFlag = false;
        }
    }

    internal class STD : Instruction
    {
        public STD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // STD - set direction flag
        protected override void ExecuteInstruction()
        {
            EU.CondReg.DirectionFlag = true;
        }
    }
}

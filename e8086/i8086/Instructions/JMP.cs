using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class JMP: ShortJumpInstruction
    {
        public JMP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override bool JumpDecision()
        {
            return true;  // unconditional jump
        }
    }

    internal class JCXZ : ShortJumpInstruction
    {
        public JCXZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override bool JumpDecision()
        {
            return (EU.Registers.CX == 0);
        }
    }

    internal class JO : ShortJumpInstruction
    {
        public JO(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.OverflowFlag;
        }
    }

    internal class JNO : ShortJumpInstruction
    {
        public JNO(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.OverflowFlag;
        }
    }

    internal class JC : ShortJumpInstruction
    {
        public JC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.CarryFlag;
        }
    }

    internal class JNC : ShortJumpInstruction
    {
        public JNC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.CarryFlag;
        }
    }

    internal class JZ : ShortJumpInstruction
    {
        public JZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.ZeroFlag;
        }
    }

    internal class JNZ : ShortJumpInstruction
    {
        public JNZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.ZeroFlag;
        }
    }

    internal class JNA : ShortJumpInstruction
    {
        public JNA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return (EU.CondReg.CarryFlag | EU.CondReg.ZeroFlag);
        }
    }

    internal class JA : ShortJumpInstruction
    {
        public JA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !(EU.CondReg.CarryFlag | EU.CondReg.ZeroFlag);
        }
    }

    internal class JS : ShortJumpInstruction
    {
        public JS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.SignFlag;
        }
    }

    internal class JNS : ShortJumpInstruction
    {
        public JNS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.SignFlag;
        }
    }

    internal class JP : ShortJumpInstruction
    {
        public JP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.ParityFlag;
        }
    }

    internal class JNP : ShortJumpInstruction
    {
        public JNP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.ParityFlag;
        }
    }

    internal class JL : ShortJumpInstruction
    {
        public JL(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return (EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag);
        }
    }

    internal class JNL : ShortJumpInstruction
    {
        public JNL(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !(EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag);
        }
    }

    internal class JNG : ShortJumpInstruction
    {
        public JNG(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return ((EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag) | EU.CondReg.ZeroFlag);
        }
    }

    internal class JG : ShortJumpInstruction
    {
        public JG(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !((EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag) | EU.CondReg.ZeroFlag);
        }
    }


}

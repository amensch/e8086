using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class JMP: ShortJumpInstruction
    {
        public JMP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override bool JumpDecision()
        {
            return true;  // unconditional jump
        }
    }

    public class JCXZ : ShortJumpInstruction
    {
        public JCXZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override bool JumpDecision()
        {
            return (EU.Registers.CX == 0);
        }
    }

    public class JO : ShortJumpInstruction
    {
        public JO(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.OverflowFlag;
        }
    }

    public class JNO : ShortJumpInstruction
    {
        public JNO(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.OverflowFlag;
        }
    }

    public class JC : ShortJumpInstruction
    {
        public JC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.CarryFlag;
        }
    }

    public class JNC : ShortJumpInstruction
    {
        public JNC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.CarryFlag;
        }
    }

    public class JZ : ShortJumpInstruction
    {
        public JZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.ZeroFlag;
        }
    }

    public class JNZ : ShortJumpInstruction
    {
        public JNZ(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.ZeroFlag;
        }
    }

    public class JNA : ShortJumpInstruction
    {
        public JNA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return (EU.CondReg.CarryFlag | EU.CondReg.ZeroFlag);
        }
    }

    public class JA : ShortJumpInstruction
    {
        public JA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !(EU.CondReg.CarryFlag | EU.CondReg.ZeroFlag);
        }
    }

    public class JS : ShortJumpInstruction
    {
        public JS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.SignFlag;
        }
    }

    public class JNS : ShortJumpInstruction
    {
        public JNS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.SignFlag;
        }
    }

    public class JP : ShortJumpInstruction
    {
        public JP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return EU.CondReg.ParityFlag;
        }
    }

    public class JNP : ShortJumpInstruction
    {
        public JNP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !EU.CondReg.ParityFlag;
        }
    }

    public class JL : ShortJumpInstruction
    {
        public JL(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return (EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag);
        }
    }

    public class JNL : ShortJumpInstruction
    {
        public JNL(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !(EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag);
        }
    }

    public class JNG : ShortJumpInstruction
    {
        public JNG(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return ((EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag) | EU.CondReg.ZeroFlag);
        }
    }

    public class JG : ShortJumpInstruction
    {
        public JG(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
        protected override bool JumpDecision()
        {
            return !((EU.CondReg.SignFlag ^ EU.CondReg.OverflowFlag) | EU.CondReg.ZeroFlag);
        }
    }


}

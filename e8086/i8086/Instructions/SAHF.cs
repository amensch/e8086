using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class SAHF : Instruction
    {
        public SAHF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // (0x9e) SAHF - store AH to flags

        protected override void ExecuteInstruction()
        {
            EU.CondReg.Value = new WordRegister((byte)(EU.CondReg.Value >> 8), EU.Registers.AH);
        }
    }
}

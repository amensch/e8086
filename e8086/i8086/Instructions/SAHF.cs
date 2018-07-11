using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class SAHF : Instruction
    {
        public SAHF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // (0x9e) SAHF - store AH to flags

        protected override void ExecuteInstruction()
        {
            EU.CondReg.Register = new DataRegister16((byte)(EU.CondReg.Register >> 8), EU.Registers.AH);
        }
    }
}

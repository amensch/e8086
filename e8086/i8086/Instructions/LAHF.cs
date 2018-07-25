using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class LAHF : Instruction
    {
        public LAHF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // (0x9f) LAHF - load AH from flags

        protected override void ExecuteInstruction()
        {
            EU.Registers.AH = (byte)(EU.CondReg.Value & 0x00ff);
        }
    }
}

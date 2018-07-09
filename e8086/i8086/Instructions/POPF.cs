using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class POPF : Instruction
    {
        /// <summary>
        /// OpCode = 0x9d POPF
        /// Pop the registers
        /// </summary>
        public POPF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            EU.CondReg.Register = Pop();
        }

    }
}

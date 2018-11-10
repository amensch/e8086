using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CWD : Instruction
    {
        public CWD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CWD - convert word to double word
        protected override void ExecuteInstruction()
        {
            if ((EU.Registers.AX & 0x8000) == 0x8000)
            {
                EU.Registers.DX = 0xffff;
            }
            else
            {
                EU.Registers.DX = 0;
            }
        }

        protected override void DetermineClocks()
        {
            Clocks = 2;
        }
    }
}

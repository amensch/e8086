using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class SALC : Instruction
    {
        public SALC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // undocumented SALC instruction
        protected override void ExecuteInstruction()
        {
            if (EU.CondReg.CarryFlag)
            {
                EU.Registers.AL = 0xff;
            }
            else
            {
                EU.Registers.AL = 0x00;
            }
        }

        public override long Clocks()
        {
            return 2;
        }

    }
}

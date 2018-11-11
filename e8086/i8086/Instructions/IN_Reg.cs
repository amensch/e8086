using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class IN_Reg : InputInstruction
    {
        public IN_Reg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            port = EU.Registers.DX;
        }

        protected override void DetermineClocks()
        {
            Clocks = 8;
        }
    }
}
